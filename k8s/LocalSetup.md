# Microservices Architecture Setup Guide

Welcome to the setup guide for our Microservices Architecture. This document provides detailed instructions on how to get our environment up and running on your local machine using Docker and Kubernetes.

## Prerequisites

Ensure you have the following tools installed:

- Docker
- Docker Desktop with Kubernetes enabled
- kubectl (cli is installed with Docker Desktop)

## Steps to Setup

### 1. Unzip the yaml files and have them locally


### 2. Pulling Docker Images

Ensure that you pull the necessary Docker images:

```bash
docker pull vragalevgeorge/sharing-economy-microservice-api-gateway:latest
docker pull vragalevgeorge/sharing-economy-microservice-inventory-service:latest
docker pull vragalevgeorge/sharing-economy-microservice-order-service:latest
```


### 3. Deploying to Kubernetes

Deploy the Kubernetes manifests:

```bash
# For the API Gateway:
kubectl apply -f apigateway-service.yaml
kubectl apply -f apigateway-deployment.yaml

# For Redis:
kubectl apply -f redis-service.yaml
kubectl apply -f redis-deployment.yaml

# For the Inventory microservice:
kubectl apply -f inventory-db-data-persistentvolumeclaim.yaml
kubectl apply -f inventory-db-service.yaml
kubectl apply -f inventory-db-deployment.yaml
kubectl apply -f inventory-service.yaml
kubectl apply -f inventory-deployment.yaml

# For the Order microservice:
kubectl apply -f order-db-data-persistentvolumeclaim.yaml
kubectl apply -f order-db-service.yaml
kubectl apply -f order-db-deployment.yaml
kubectl apply -f order-service.yaml
kubectl apply -f order-deployment.yaml
```

If you want to remove the pods, you can use the following:

Delete the Kubernetes manifests:

```bash
# For the API Gateway:
kubectl delete -f apigateway-deployment.yaml
kubectl delete -f apigateway-service.yaml

# For Redis:
kubectl delete -f redis-deployment.yaml
kubectl delete -f redis-service.yaml

# For the Inventory microservice:
kubectl delete -f inventory-deployment.yaml
kubectl delete -f inventory-service.yaml
kubectl delete -f inventory-db-deployment.yaml
kubectl delete -f inventory-db-service.yaml
kubectl delete -f inventory-db-data-persistentvolumeclaim.yaml

# For the Order microservice:
kubectl delete -f order-deployment.yaml
kubectl delete -f order-service.yaml
kubectl delete -f order-db-deployment.yaml
kubectl delete -f order-db-service.yaml
kubectl delete -f order-db-data-persistentvolumeclaim.yaml
```

### 4. Accessing the Services

#### API Gateway

Since we are using local deployment, we'll use port-forward to access the API Gateway:

```bash
kubectl port-forward svc/api-gateway-service 5000:5000
```

You can now access the API Gateway on [http://localhost:5000/](http://localhost:5000/).

#### !!! Use the attached Postman collection to test. !!!

## Monitoring

Make sure to monitor logs and the health of the services. You can use:

```bash
kubectl logs [POD_NAME]
```

And:

Once the services are ready all the pods should be in the running state after running:
```bash
kubectl get pods
```

To check the status of the pods.



