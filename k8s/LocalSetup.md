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
docker pull your_dockerhub_username/api-gateway:tag
docker pull your_dockerhub_username/inventory-service:tag
docker pull your_dockerhub_username/order-service:tag
```

Replace `your_dockerhub_username` with the actual Docker Hub username and use the appropriate tags.

### 3. Deploying to Kubernetes

Deploy the Kubernetes manifests:

```bash
kubectl apply -f ./path_to_k8s_manifests/
```

Replace `path_to_k8s_manifests` with the directory containing the Kubernetes YAML files.

### 4. Accessing the Services

#### API Gateway

Since we are using local deployment, we'll use port-forward to access the API Gateway:

```bash
kubectl port-forward service/api-gateway-service 5000:5000
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
