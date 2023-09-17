from flask import Flask, request, jsonify
import requests
import redis
from threading import Semaphore
from hashlib import sha256
from consul import Consul

app = Flask(__name__)

# Initialize Redis
r = redis.Redis(host='localhost', port=6379, db=0)

# Initialize Semaphore for limiting concurrency
sem = Semaphore(10)  # Allow 10 concurrent tasks at max

# Initialize Consul client
consul = Consul(host='localhost', port=8500)

def get_service_url(service_name):
    index, data = consul.catalog.service(service_name)
    if data:
        service = data[0]
        return f'http://{service["ServiceAddress"]}:{service["ServicePort"]}'
    else:
        return None

def cache_key(action, payload):
    """Generate a cache key by hashing the action and payload."""
    return sha256(f"{action}{str(payload)}".encode()).hexdigest()

@app.route('/user/<action>', methods=['POST'])
def user_service(action):
    service_url = get_service_url('user-service')
    if not service_url:
        return 'Service not found', 404

    response = requests.post(f'{service_url}/{action}', json=request.json)
    return (response.text, response.status_code, response.headers.items())

@app.route('/trailer/<action>', methods=['POST'])
def trailer_service(action):
    auth_key = request.headers.get("Authorization")

    user_service_url = get_service_url('user-service')
    if not user_service_url:
        return 'User Service not found', 404

    # Validate the key with User service
    auth_response = requests.post(f'{user_service_url}/validate', headers={'Authorization': auth_key})
    if auth_response.status_code != 200:
        return 'Unauthorized', 401

    # Generate cache key
    key = cache_key(action, request.json)

    # Try fetching the result from cache
    cached_result = r.get(key)
    if cached_result:
        return jsonify({"result": cached_result.decode("utf-8"), "source": "cache"})

    with sem:
        task_id = f"task:{action}"

        # Check if task is within concurrency limit and timeouts
        if r.exists(task_id):
            return 'Too Many Requests', 429

        r.setex(task_id, 5, 'running')  # 5-second timeout

        trailer_service_url = get_service_url('trailer-service')
        if not trailer_service_url:
            return 'Trailer Service not found', 404

        # Forward the request to the Trailer service
        response = requests.post(f'{trailer_service_url}/{action}', json=request.json)

        if response.status_code == 200:
            # Cache the result with a TTL of 60 seconds
            r.setex(key, 60, response.text)

        # Remove the task from Redis to signal completion
        r.delete(task_id)

    return (response.text, response.status_code, response.headers.items())

if __name__ == '__main__':
    app.run(debug=True, port=5000)
