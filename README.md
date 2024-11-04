# Web API Project Overview

This is a Web API project built on .NET 8, following Clean Architecture principles. The objective of the project is to provide standardized APIs with features such as security, monitoring, health checks, efficient database management, and integration with external services.

## Architecture

The project follows Clean Architecture with distinct layers:

- **Domain**: Contains entities, business rules, and interfaces.
- **Application**: Contains application logic and command handling via Mediator.
- **Infrastructure**: Provides services and connections to databases, Kafka, Redis, and other external services.
- **API**: The layer that interacts with users through APIs.

## Technologies Used

1. **Authentication**
   - Uses JWT (JSON Web Tokens) for user authentication.
   - API authentication is handled via API-Key.

2. **Authorization**
   - Access control is managed via role-based authorization.

3. **Traces, Metrics, Logs**
   - OpenTelemetry is used to collect tracing and metrics data.
   - Kafka is used for logging, enabling real-time log processing.

4. **Health Check**
   - Implements health checks for the main service and auxiliary services such as databases (SQL, MongoDB) and Kafka via health check endpoints.

5. **Response Object**
   - Utilizes a standardized response object containing status, message, and return data, adhering to the "API Documentation Core System Standard v1.0" specification.

6. **Common Functions**
   - Provides utility functions such as:
     - Retrieving user information from the context.
     - Reading/Writing JSON, Excel files, etc.
     - Date formatting.

7. **Database Connectivity**
   - Supports SQL, Redis, and MongoDB with features like connection pooling, timeout management, and retry policies.

8. **API Versioning**
   - Supports versioning to maintain backward compatibility when extending APIs.

9. **Global Exception Handling**
   - Implements global exception handling to ensure the system remains stable without abrupt failures.

10. **API Call Handling**
    - Uses HttpClientFactory to manage HTTP connections.
    - Polly is used to configure retry policies and circuit breaker mechanisms.

11. **Validation**
    - FluentValidation is used to validate incoming request data.

12. **Configuration**
    - Manages configuration securely using Vault.

13. **Caching**
    - Implements caching for resources like products usin
