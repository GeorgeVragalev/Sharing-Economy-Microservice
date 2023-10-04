

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

#### Order service
The order service will have default crud operations as well as 
operations related to ordering and reserving items from inventory.

The order service will receive a reservation request from the api gateway and first try to reserve the item from inventory service, if the request is successful then we add a record for the order.

#### Inventory service
The inventory service will have default crud operations. The inventory service will receive requests from order service to reserve items and place orders that are linked to those items for that user.

### Project design

The essense of the project is to place orders using orders service and reserve items from inventory.

![Local Image](./Architecture_Diagram.jpg)

#### How it works


## Communication Flow:

	1.	API Gateway: The central entry point for all external requests. It’s responsible for request routing, composition, and other cross-cutting concerns like caching. When it receives a request for a specific service, it forwards the request to the corresponding service.
	2.	Service Discovery: The API Gateway has a service registry that holds the URL of each service. This is a simplified form of service discovery, which enables the API Gateway to know where to route incoming requests.
	3.	Request Forwarding: After determining the destination based on the service name in the URL, the API Gateway forwards the HTTP request to the corresponding service (either Inventory or Order service in your setup).
	4.	Database Interaction: Each service (Inventory and Order) has its PostgreSQL database. When a service receives a request, it reads from or writes to its database as needed.
	5.	Response: After processing, the service sends a response back to the API Gateway, which in turn sends it back to the client.
	6.	Caching: Redis is used to cache responses for specific keys. Before the API Gateway forwards a request to a service, it checks if the result is already cached. If so, it returns the cached result.

## Data Exchange:

	* HTTP/JSON: Data is exchanged in JSON format over HTTP. Your services seem to be RESTful, using HTTP GET and POST methods.
	* Query Parameters and JSON Payload: For GET requests, query parameters can be forwarded as is. For POST requests, a JSON payload is forwarded.
	* Concurrent Requests: The API Gateway uses a semaphore (sem in your code) to limit the number of concurrent requests.
	* Timeouts: You’ve implemented request timeouts using Python’s requests library, ensuring that requests don’t hang indefinitely.
	* Error Handling: Proper logging and HTTP error codes are returned for various error cases (service not found, bad response from a service, timeout).

## Database:

	* Isolated Databases: Each service has its PostgreSQL database running in its container.
	* Connection Strings: The database connection information is provided through environment variables.

Overall, the system is designed to be decoupled, with services knowing as little as necessary about each other, thereby adhering to microservices best practices.