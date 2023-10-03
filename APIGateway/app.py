from flask import Flask, request, jsonify
import requests
import redis
from threading import Semaphore
from hashlib import sha256
import logging

app = Flask(__name__)

# Initialize Redis
r = redis.Redis(host='host.docker.internal', port=6379, db=0)

# Initialize Semaphore for limiting concurrency
sem = Semaphore(10)

# Service registry for local service discovery
service_registry = {
    "inventory": "http://inventory-service:80/api/inventory",
    "order": "http://order-service:80/api/order",
}

def discover_service(service_name):
    return service_registry.get(service_name, None)


def cache_key(action, payload):
    """Generate a cache key by hashing the action and payload."""
    return sha256(f"{action}{str(payload)}".encode()).hexdigest()


@app.route('/status')
def index():
    return 'Api gateway working on port: 5000'


@app.route('/api/<service>/<action>', methods=['GET', 'POST'])
def generic_service(service, action):
    try:
        # Your existing service discovery logic here
        service_url = discover_service(service)
        if service_url is None:
            logging.error(f'Service {service} not found')
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
            else:
                logging.error(f'Bad response from service: {response.status_code}, {response.text}')
                return jsonify({"error": f"Bad response from service: {response.status_code}"}), response.status_code

        return response.text, response.content

    except Exception as e:
        message = f'An error occurred: {str(e)}'
        logging.error(message)
        return jsonify({"error": message}), 500


if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)
