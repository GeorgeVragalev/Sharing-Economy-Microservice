import json
import logging
import time
from collections import deque
from hashlib import sha256
import pybreaker
import requests
from flask import Flask, request, jsonify, Response
from flask_limiter import Limiter
from flask_limiter.util import get_remote_address
import prometheus_client
from prometheus_client import Counter, Gauge, Histogram
from requests.exceptions import ConnectionError
from requests.exceptions import Timeout
import hazelcast

# Initialize Flask app
app = Flask(__name__)

limiter = Limiter(
    app,
    default_limits=["50 per minute"]  # Adjust these values as required
)
limiter.request_filter = get_remote_address

isDeployment = True
cache_link = 'hazelcast' if isDeployment else 'localhost'
inventory_link = 'inventory-service:80' if isDeployment else 'localhost:5217'
order_link = 'order-service:80' if isDeployment else 'localhost:5143'

# Initialize Hazelcast Client
client = hazelcast.HazelcastClient(
    cluster_name="dev",
    cluster_members=[
        f"{cache_link}:5701"
    ]
)
hz_map = client.get_map("api-cache").blocking()

# Initialize Circuit Breaker
re_route_counter = {}
REROUTE_THRESHOLD = 10
breaker = pybreaker.CircuitBreaker(fail_max=REROUTE_THRESHOLD, reset_timeout=60)

# Initialize Prometheus metrics
cache_hits = 0
cache_misses = 0
CACHE_HITS = Counter('api_gateway_cache_hits', 'Total number of cache hits')
CACHE_MISSES = Counter('api_gateway_cache_misses', 'Total number of cache misses')
CACHE_HIT_RATE = Gauge('api_gateway_cache_hit_rate', 'Cache hit rate')
REQUEST_COUNTER = Counter('api_gateway_total_requests', 'Total number of requests handled by the api gateway', ['method', 'endpoint'])
REQUEST_LATENCY = Histogram('api_gateway_request_latency_seconds', 'Histogram of latencies for requests', ['method', 'endpoint'])
ERROR_REQUESTS = Counter('api_gateway_error_requests', 'Total number of error requests', ['method', 'endpoint', 'http_status'])

# Service registry for local service discovery
service_registry = {
    "inventory": deque([f"http://{inventory_link}/api/inventory"]),
    "order": deque([f"http://{order_link}/api/order"]),
}


def handle_json_response(response):
    try:
        # Attempt to deserialize the content as JSON
        parsed_data = json.loads(response)
        # Serialize it again with indentation
        formatted_data = json.dumps(parsed_data, indent=4)
        return formatted_data
    except json.JSONDecodeError:
        logging.error("Failed to decode json. Returning raw response")
        return response


def discover_service(service_name):
    if service_name not in service_registry:
        return None

    instance = service_registry[service_name].popleft()
    service_registry[service_name].append(instance)

    # Increment the re-route counter
    if service_name not in re_route_counter:
        re_route_counter[service_name] = 0

    return instance


def cache_key(action, payload):
    return sha256(f"{action}{str(payload)}".encode()).hexdigest()


@app.errorhandler(429)
def ratelimit_error(e):
    return jsonify(error="ratelimit exceeded", message=str(e.description)), 429


@app.route('/metrics')
def metrics():
    return Response(prometheus_client.generate_latest(), mimetype=str('text/plain; version=0.0.4; charset=utf-8'))


@app.route('/status')
def status():
    return 'STATUS: OK. API gateway port: 5000'


@app.route('/clear_cache')
def clear_cache():
    try:
        hz_map.clear()
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
        # Increment the request counter
        REQUEST_COUNTER.labels(request.method, f"/api/{service}/{action}").inc()

        start_time = time.time()  # Start time for latency measurement

        # Check the re-route counter
        if re_route_counter.get(service, 0) > REROUTE_THRESHOLD:
            raise pybreaker.CircuitBreakerError

        service_url = discover_service(service)
        if service_url is None:
            logging.error(f'Service {service} not found')
            return jsonify({"error": "Service not found"}), 404

        if request.method in ['POST', 'PUT', 'DELETE']:
            hz_map.delete(cache_key(service, action))

        key = cache_key(action, request.args)

        if request.method == 'GET':
            logging.info("Checking Cache")
            cached_result = hz_map.get(key)
            if cached_result is not None:
                logging.info("Using Cache")

                update_cache_metrics(True)

                # Record request latency
                REQUEST_LATENCY.labels(request.method, f"/api/{service}/{action}").observe(time.time() - start_time)

                return handle_json_response(cached_result)
            else:
                update_cache_metrics(False)

        response = perform_request(f'{service_url}/{action}', request.method, params=request.args)

        # Record request latency
        REQUEST_LATENCY.labels(request.method, f"/api/{service}/{action}").observe(time.time() - start_time)

        if 200 <= response.status_code < 300:
            if request.method == 'GET':
                hz_map.set(key, response.text, ttl=60)  # Cache the new result
        else:
            re_route_counter[service] += 1

            ERROR_REQUESTS.labels(request.method, f"/api/{service}/{action}", response.status_code).inc()
            logging.error(f'Bad response from service: {response.status_code}, {response.text}')

        if response.status_code < 500:
            re_route_counter[service] = 0

        formatted_json = handle_json_response(response.text)
        return formatted_json, response.status_code

    except ConnectionError:
        logging.error('Connection error occurred. The service might be down.')
        return handle_exception_response(action, service, "Service might be down. Connection error occurred.", 503)

    except Timeout:
        logging.error(f'Timeout occurred while accessing {service}/{action}')
        return handle_exception_response(action, service, "Request timed out", 408)

    except pybreaker.CircuitBreakerError:
        logging.error('Circuit breaker is open')
        return handle_exception_response(action, service, "Circuit is open; failing fast", 503)

    except Exception as e:
        logging.error(f'An error occurred: {str(e)}')
        return handle_exception_response(action, service, f"An unexpected error occurred. {str(e)}", 500)


def handle_exception_response(action, service, message, status_code):
    ERROR_REQUESTS.labels(request.method, f"/api/{service}/{action}", status_code).inc()
    return jsonify({"error": message}), status_code


def update_cache_metrics(is_hit):
    global cache_hits, cache_misses

    if is_hit:
        CACHE_HITS.inc()
        cache_hits += 1
    else:
        CACHE_MISSES.inc()
        cache_misses += 1

    total_requests = cache_hits + cache_misses
    hit_rate = cache_hits / total_requests if total_requests > 0 else 0
    CACHE_HIT_RATE.set(hit_rate)


if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)