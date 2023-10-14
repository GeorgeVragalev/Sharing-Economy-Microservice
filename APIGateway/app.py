from flask import Flask, request, jsonify
import requests
import redis
from hashlib import sha256
import logging
import pybreaker
from requests.exceptions import Timeout
from collections import deque
from flask_limiter import Limiter
from flask_limiter.util import get_remote_address
from requests.exceptions import ConnectionError


# Initialize Flask app
app = Flask(__name__)

limiter = Limiter(
    app,
    default_limits=["50 per minute"]  # Adjust these values as required
)
limiter.request_filter = get_remote_address


isDeployment = True
redis_link = 'redis-service' if isDeployment else 'localhost'
# Initialize Redis
r = redis.Redis(host=redis_link, port=6379, db=0)

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


@app.errorhandler(429)
def ratelimit_error(e):
    return jsonify(error="ratelimit exceeded", message=str(e.description)), 429


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
def perform_request(url, method, params=None):
    if method == 'GET':
        return requests.get(url, params=params, timeout=5)
    elif method == 'POST':
        return requests.post(url, json=request.json, timeout=5)
    elif method == 'PUT':
        return requests.put(url, json=request.json, timeout=5)
    elif method == 'DELETE':
        return requests.delete(url, timeout=5)


@app.route('/api/<service>/<action>', methods=['GET', 'POST', 'PUT', 'DELETE'])
def generic_service(service, action):
    try:
        service_url = discover_service(service)
        if service_url is None:
            logging.error(f'Service {service} not found')
            return jsonify({"error": "Service not found"}), 404

        if request.method in ['POST', 'PUT', 'DELETE']:
            r.delete(cache_key(service, action))

        key = cache_key(action, request.args)

        if request.method == 'GET':
            cached_result = r.get(key)
            if cached_result:
                return jsonify({"result": cached_result.decode("utf-8"), "source": "cache"})

        response = perform_request(f'{service_url}/{action}', request.method, params=request.args)
        if response.status_code == 200:
            if request.method == 'GET':
                r.setex(key, 60, response.text)
            return response.text, response.content
        else:
            logging.error(f'Bad response from service: {response.status_code}, {response.text}')
            return jsonify({"error": f"Bad response from service: {response.status_code}"}), response.status_code

    except ConnectionError:
        logging.error('Connection error occurred. The service might be down.')
        return jsonify({"error": "Service might be down. Connection error occurred."}), 503

    except Timeout:
        logging.error(f'Timeout occurred while accessing {service}/{action}')
        return jsonify({"error": "Request timed out"}), 408

    except pybreaker.CircuitBreakerError:
        logging.error('Circuit breaker is open')
        return jsonify({"error": "Circuit is open; failing fast"}), 503

    except Exception as e:
        logging.error(f'An error occurred: {str(e)}')
        return jsonify({"error": f"An unexpected error occurred. {str(e)}"}), 500


if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)