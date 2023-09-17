from flask import Flask, request, jsonify
import requests
import redis
from threading import Semaphore
from hashlib import sha256

app = Flask(__name__)

# Initialize Redis
r = redis.Redis(host='localhost', port=6379, db=0)  # Assume Redis is running as 'redis-service' in Kubernetes

# Initialize Semaphore for limiting concurrency
sem = Semaphore(10)


def cache_key(action, payload):
    """Generate a cache key by hashing the action and payload."""
    return sha256(f"{action}{str(payload)}".encode()).hexdigest()


@app.route('/api/<service>/<action>', methods=['GET', 'POST'])
def generic_service(service, action):
    service_url = f"http://{service}-service:8080"  # Use DNS resolution in Kubernetes

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

    return (response.text, response.status_code, response.headers.items())


if __name__ == '__main__':
    app.run(debug=True, port=5000)
