from flask import Flask, request, jsonify
import requests
import redis
from threading import Semaphore
from hashlib import sha256

app = Flask(__name__)

# Initialize Redis
r = redis.Redis(host='localhost', port=6379, db=0)

# Initialize Semaphore for limiting concurrency
sem = Semaphore(10)

# Service registry for local service discovery
service_registry = {
    "user": "http://localhost:5148/api/user",
    "inventory": "http://localhost:5149/api/catalog",
}

def discover_service(service_name):
    return service_registry.get(service_name, None)

def cache_key(action, payload):
    """Generate a cache key by hashing the action and payload."""
    return sha256(f"{action}{str(payload)}".encode()).hexdigest()

@app.route('/api/<service>/<action>', methods=['GET', 'POST'])
def generic_service(service, action):
    service_url = discover_service(service)
    if service_url is None:
        return jsonify({"error": "Service not found"}), 404

    key = cache_key(action, request.json if request.method == 'POST' else request.args)

    # Try fetching the result from cache
    cached_result = r.get(key)
    if cached_result:
        return jsonify({"result": cached_result.decode("utf-8"), "source": "cache"})

    with sem:
        if request.method == 'GET':
            response = requests.get(f'{service_url}/{action}', params=request.args)
        else:  # POST
            response = requests.post(f'{service_url}/{action}', json=request.json)

        if response.status_code == 200:
            # Cache the result with a TTL of 60 seconds
            r.setex(key, 60, response.text)

    return response.text, response.content

if __name__ == '__main__':
    app.run(debug=True, port=5000)