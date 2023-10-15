# Microservices Architecture Setup Guide

Welcome to the setup guide for our Microservices Architecture. This document provides detailed instructions on how to get our environment up and running on your local machine using Docker and Kubernetes.

## Prerequisites

Ensure you have the following tools installed:

- Docker
- Docker Desktop with Kubernetes enabled or Minikube
- kubectl

## Steps to Setup

### 1. Clone the Repository (if shared via Git)

```bash
git clone [Repository URL]
cd [Repository Folder Name]
```

Replace `[Repository URL]` with the actual repository URL and `[Repository Folder Name]` with the cloned folder's name.

### 2. Pulling Docker Images

Ensure that you pull the necessary Docker images:

```bash
docker pull vragalevgeorge/sharing-economy-microservice-api-gateway:latest
docker pull vragalevgeorge/sharing-economy-microservice-inventory-service:latest
docker pull vragalevgeorge/sharing-economy-microservice-order-service:latest
```

Replace `your_dockerhub_username` with the actual Docker Hub username and use the appropriate tags.

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

## Scaling and Other Notes

If you want to scale any service, you can use the following:

```bash
kubectl scale deployment [DEPLOYMENT_NAME] --replicas=[NUMBER]
```

Replace `[DEPLOYMENT_NAME]` with the deployment's name and `[NUMBER]` with the desired number of replicas.

Make sure to monitor logs and the health of the services. You can use:

```bash
kubectl logs [POD_NAME]
```

And:

```bash
kubectl get pods
```

To check the status of the pods.



