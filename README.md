

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
