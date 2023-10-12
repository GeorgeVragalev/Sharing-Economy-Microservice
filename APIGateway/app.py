from flask import Flask, request, jsonify
import requests
import redis
from threading import Semaphore
from hashlib import sha256
import logging
import pybreaker
from requests.exceptions import Timeout
from collections import deque

# Initialize Flask app
app = Flask(__name__)

# Initialize Redis
r = redis.Redis(host='host.docker.internal', port=6379, db=0)

# Initialize Semaphore for limiting concurrency
sem = Semaphore(10)

# Initialize Circuit Breaker
breaker = pybreaker.CircuitBreaker(fail_max=3, reset_timeout=60)

# Service registry for local service discovery
service_registry = {
    "inventory": deque(["http://inventory-service:80/api/inventory"]),
    "order": deque(["http://order-service:80/api/order"]),
}


def discover_service(service_name):
    if service_name not in service_registry:
        return None

    instance = service_registry[service_name].popleft()
    service_registry[service_name].append(instance)
    return instance


def cache_key(action, payload):
    return sha256(f"{action}{str(payload)}".encode()).hexdigest()


@app.route('/status')
def status():
    return 'STATUS: OK. API gateway port: 5000'


@app.route('/clear_cache')
def clear_cache():
    try:
        r.flushall()
        return 'Flushed all cache keys', 200
    except Exception as e:
        return f"An error occurred: {str(e)}", 500


@breaker
def perform_request(url, method, params=None, json=None):
    if method == 'GET':
        return requests.get(url, params=params, timeout=5)
    else:
        return requests.post(url, json=json, timeout=5)


@app.route('/api/<service>/<action>', methods=['GET', 'POST'])
def generic_service(service, action):
    try:
        service_url = discover_service(service)
        if service_url is None:
            logging.error(f'Service {service} not found')
            return jsonify({"error": "Service not found"}), 404

        key = cache_key(action, request.json if request.method == 'POST' else request.args)
        cached_result = r.get(key)
        if cached_result:
            return jsonify({"result": cached_result.decode("utf-8"), "source": "cache"})

        with sem:
            response = perform_request(f'{service_url}/{action}', request.method, params=request.args, json=request.json)
            if response.status_code == 200:
                r.setex(key, 60, response.text)
                return response.text, response.content
            else:
                logging.error(f'Bad response from service: {response.status_code}, {response.text}')
                return jsonify({"error": f"Bad response from service: {response.status_code}"}), response.status_code

    except Timeout:
        logging.error(f'Timeout occurred while accessing {service}/{action}')
        return jsonify({"error": "Request timed out"}), 408

    except pybreaker.CircuitBreakerError:
        logging.error('Circuit breaker is open')
        return jsonify({"error": "Circuit is open; failing fast"}), 503

    except Exception as e:
        logging.error(f'An error occurred: {str(e)}')
        return jsonify({"error": "An unexpected error occurred"}), 500


if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)