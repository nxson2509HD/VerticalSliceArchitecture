TechShop Web API
Overview
This is a Web API project built on .NET 8, following Clean Architecture principles. The objective of the project is to provide standardized APIs with features such as security, monitoring, health checks, efficient database management, and integration with external services.

Architecture
The project follows Clean Architecture with distinct layers:

Domain: Contains entities, business rules, and interfaces.
Application: Contains application logic and command handling via Mediator.
Infrastructure: Provides services and connections to databases, Kafka, Redis, and other external services.
API: The layer that interacts with users through APIs.
Technologies Used
1. Authentication
Uses JWT (JSON Web Tokens) for user authentication.
API authentication is handled via API-Key.
2. Authorization
Access control is managed via role-based authorization.
3. Traces, Metrics, Logs
OpenTelemetry is used to collect tracing and metrics data.
Kafka is used for logging, enabling real-time log processing.
4. Health Check
Implements health checks for the main service and auxiliary services such as databases (SQL, MongoDB), Kafka via health check endpoints.
5. Response Object
Utilizes a standardized response object containing status, message, and return data. This structure adheres to the "API Documentation Core System Standard v1.0" specification.
6. Common Functions
Provides utility functions such as:
Retrieving user information from the context.
Reading/Writing JSON, Excel files, etc.
Date formatting.
7. Database Connectivity
Supports SQL, Redis, and MongoDB with features like connection pooling, timeout management, and retry policies.
8. API Versioning
Supports versioning to maintain backward compatibility when extending APIs.
9. Global Exception Handling
Implements global exception handling to ensure the system remains stable without abrupt failures.
10. API Call Handling
Uses HttpClientFactory to manage HTTP connections.
Polly is used to configure retry policies and circuit breaker mechanisms.
11. Validation
FluentValidation is used to validate incoming request data.
12. Configuration
Manages configuration securely using Vault.
13. Caching
Implements caching for resources like products using Redis.
14. Rate Limiting
Implements rate limiting to ensure system stability under high load.
15. Swagger
Swagger is used to generate API documentation and provide an interactive API testing interface.
16. Kafka
Connects to Kafka for both producer and consumer functionality in the PubSub system.
17. Dynamic Expression
Supports Dynamic Expression for executing dynamic rules.
Infrastructure
The project uses Docker for deploying the environment, ensuring consistency and easy management of services.

Installation and Running
Developer Mode Setup
1. Create the Database
Open a terminal and navigate to the project root.

Navigate to the TechShopSolution.Infrastructure folder:

cd TechShopSolution.Infrastructure
Run the following command to create a new migration for the database schema:

dotnet ef migrations add InitDatabaseMigration --startup-project ../TechShopSolution.API/
After running the migration command, ensure the system creates the database named techshop. This can typically be done by running:

dotnet ef database update --startup-project ../TechShopSolution.API/
2. Docker Setup
Open a terminal and navigate to the upgrade-dev-infras folder by running:

cd upgrade-dev-infras
Start the Docker infrastructure by running:

docker compose up -d
This will set up the necessary services like Kafka, Redis, and MongoDB.

3. Build and Run the API
Navigate to the TechShopSolution.API folder:

cd ../TechShopSolution.API
Build the API project:

dotnet build
Run the API:

dotnet run
API should now be running in developer mode with the necessary infrastructure services provided by Docker and the TechShop database created.

Documentation
Refer to API Documentation Core System Standard v1.0 for detailed API response structure and other standards.