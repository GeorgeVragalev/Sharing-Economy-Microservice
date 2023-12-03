

## Sharing Economy Platform for sharing items



### Project relevance
#### Complexity and Independence:

The application seems to involve multiple services, like User and Inventory, that have distinct responsibilities. Microservices allow for isolating the complexity of each service.
The components can be developed, deployed, scaled, and maintained independently, allowing for more agility in development and deployment cycles.
#### Scalability:

Each service can be scaled independently based on its needs and demand, which is essential for efficiently utilizing resources and managing costs, especially in a cloud-native environment.
#### Technology Stack Flexibility:

Microservices enable the use of different technology stacks for different services (e.g., Python for API Gateway, and C# .Net for user and inventory services), allowing the selection of the most suitable technologies for each service’s needs.
#### Resilience and Fault Isolation:

In a monolithic architecture, a failure in one component can potentially bring down the entire application. Microservices architecture helps in isolating faults to the affected service, reducing the blast radius of failures and improving overall system resilience.

### Service boundaries

#### User service
The user service will have default crud operations as well as
operations related to authentication such as login, register and validate token.

The validate token endpoint will be user by the Inventory service in order to validate the user to perform certain actions.

#### Inventory service
The inventory service will have default crud operations. The get endpoints will be publicly available.
However the post, put delete endpoints will have to accept a verification token, which will be sent from the user from headers, and will user the validate-token
endpoint of the user service to authorize the user permission to change or insert entities.

### Project design

The essense of the project is to allow only authorized users to specific operations for inventory management.

![Local Image](./Architecture_Diagram.jpg)

#### How it works

1. Firstly we need to register a user so we can call the register endpoint.
2. Then we need to authorize the user so we login and the login in stored in cache so you do not log in twice for example.
3. Then using this login token the user can call the other inventory microservice to perform curd operations on the entities.
4. The user can access the inventory items without the token as get requests will be public.
5. Once a request is sent to inventory microservice, it calls the user microservice to valdiate the token and if the token is valid then the request can be performed.
6. The request is returned and is stored in cache.

Service discovery and load balancing as well as regular health checks will eventually be implemented using kubernetes as it is convenient to use an all in one platform

For the service load I might consider other approaches for load balancing.

All requests will be sent using http protocols.


To run run the docker compose
to create script for migrations dotnet ef migrations script > output.sql
Connect to dbs and apply each migration manually
then you can make requests

# Local Setup

### Unzip locally the zipped file from the repository

It contains the kubernetes configuration files that have to be applied

Also it contains the LocalSetup.md file which contains the instructions to run the application locally

### Follow the instructions in the LocalSetup.md file


## Lab 2



#### 1. Circuit Breaker

The Circuit Breaker pattern is essential to prevent repeated failures, giving the affected 
component time to recover. Think of it as an electrical circuit breaker: it stops the flow of 
requests when a fault is detected. 

Implementation:

I've implemented the circuit breaker in our Python API gateway. 
By leveraging libraries like CircuitBreaker for Python, I've wrapped calls to external services. 
When the number of failures crosses a certain threshold, the breaker will trip, and further 
calls will be automatically failed for a set duration.




#### 2. Service High Availability:
High availability ensures that our service remains accessible without interruptions. 
By minimizing downtime, we ensure our service remains continuously operational.

Implementation:
Kubernetes provides an inherent high availability mechanism. Using deployments, I've specified
the desired replica count for our services. If any pod encounters issues, Kubernetes will
ensure another starts, maintaining our desired service count.


#### 3. Consistent Hashing for Cache:
The primary goal here is to distribute cache entries across multiple nodes efficiently, 
ensuring minimal rehashing when nodes are either added or removed.

Implementation:
For our cache, Redis in cluster mode has been selected, which employs consistent hashing to distribute keys across various nodes.


#### 4. Cache High Availability:
Cache High Availability ensures our caching service remains up and running, even if some nodes fail.
This setup is crucial for preserving our cached data and maintaining optimal performance.

Implementation:
I've set up Redis in a high availability configuration within Kubernetes. 



#### 4. Logging with Prometheus and Grafana:
For our system's optimal operation, logging and monitoring are indispensable. 
It allows us to observe, debug, and improve our services effectively.

Implementation:
Prometheus and Grafana have been deployed on our Kubernetes cluster. 
While Prometheus fetches metrics from our services, Grafana is responsible for visualizing these metrics.
All our services have been configured to expose relevant metrics in a format Prometheus comprehends.


#### 5. Long-running saga transactions:
The goal is to manage business transactions that span a longer duration without resorting to 2PC. 
Instead, we break them into isolated steps or sagas.

Implementation:
Here's how I've broken down an example transaction:

Our API gateway sends a request to the inventory service to reserve an item.
Upon item availability, the transaction proceeds to initiate payment and place the order.
After payment confirmation, the item is tagged as "in use". 
Should any step fail, compensating transactions are triggered to reverse prior actions.


#### 6. Database redundancy/replication + failover:
This ensures data integrity and uninterrupted availability, even in the face of database node failures.

Implementation:
For our setup, I've integrated the Zalando Postgres operator within Kubernetes. 
This operator streamlines PostgreSQL tasks like failover and backup, ensuring multiple 
database replicas for high availability.


#### 7. Data Warehousing:
A data warehouse serves as a consolidated repository for integrated data from various sources. 
This system facilitates business intelligence activities, particularly analytics.

Implementation:
I've opted for [Google BigQuery/Oracle Cloud's free tier] or any one of them that is free.
Post-setup, an ETL (Extract, Transform, Load) process has been established. 
It periodically pulls data from our microservices' databases, updating our data warehouse.


### Setting up cache on docker
Create the files in the volumes with full permissions and map them to the docker directory
```bash
New-Item -ItemType Directory -Force -Path .\redis-data\node1
New-Item -ItemType Directory -Force -Path .\redis-data\node2
New-Item -ItemType Directory -Force -Path .\redis-data\node3

New-Item -ItemType File -Force -Path .\redis-data\node1\nodes.conf
New-Item -ItemType File -Force -Path .\redis-data\node2\nodes.conf
New-Item -ItemType File -Force -Path .\redis-data\node3\nodes.conf

icacls .\redis-data\node1\nodes.conf /grant Everyone:F
icacls .\redis-data\node2\nodes.conf /grant Everyone:F
icacls .\redis-data\node3\nodes.conf /grant Everyone:F
```

Initialize the redis cluster
```bash
docker exec -it redis-node1 redis-cli --cluster create redis-node1:6379 redis-node2:6379 redis-node3:6379 --cluster-replicas 0
```

Inspect the network to check the ip addresses of the nodes
```bash
docker network inspect  sharing-economy-microservice_shared_app_network
```

Get the cluster nodes status
```bash
docker exec -it redis-node1 redis-cli -c -p 6379 cluster nodes
```