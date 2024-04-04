
# MTask Tag Management Service

The MTask Tag Management Service is a .NET 8 web application designed for fetching, processing, and managing tag data from the StackExchange API. Utilizing Entity Framework Core for data access and Serilog for comprehensive logging, this service offers efficient tag management and data analytics capabilities.

## Features

- **Data Fetching and Processing**: Automates the retrieval and saving of tags from the StackExchange API, including calculating and assigning a percentage for each tag's count in the whole tag population.
- **Data Management**: Supports operations for sorting, paging, and refreshing the tag data, facilitating easy access and management of tag information.
- **Advanced Logging**: Implements Serilog for advanced logging capabilities, including console and file sinks for effective monitoring and debugging.

## For recrutation examination

Please note that I am aware of not splitting this task into separate projects: LOGIC, DATA, API - everything except for tests is inserted as one project with divided folders. 
This is a deliberate move to simplify the structure of such a small application.
The lack of TagRepository and DTOs has also been omitted following the KISS principle.

## Testing

The project employs NUnit for unit testing and Moq for dependency mocking.

## Loging

Configured with Serilog, the service logs detailed information to both the console and file,
aiding in troubleshooting and operational monitoring. The logging setup is customizable via the appsettings.json file, allowing for adjustments to log levels and outputs as needed.

## Swagger Integration

For easy API exploration and interaction, Swagger is integrated into the service. Once the application is up and running,
the Swagger UI can be accessed at the default URL /swagger, providing a user-friendly interface for testing API endpoints.

## Dependencies

The project leverages several key packages to provide its functionality, including:

    Microsoft.EntityFrameworkCore
    Serilog.AspNetCore
    Swashbuckle.AspNetCore
    Npgsql.EntityFrameworkCore.PostgreSQL
    NUnit, Moq for testing


## Tools and Packages

- .NET 8.0
- Microsoft.EntityFrameworkCore 8.0.3
- Microsoft.EntityFrameworkCore.Design 8.0.3
- Microsoft.EntityFrameworkCore.InMemory 8.0.3
- Moq 4.20.70
- Npgsql.EntityFrameworkCore.PostgreSQL 8.0.2
- Npgsql.EntityFrameworkCore.PostgreSQL.Design 1.1.0
- NUnit 4.1.0
- Serilog.AspNetCore 8.0.1
- Serilog.Sinks.Console 5.0.1
- Serilog.Sinks.File 5.0.0
- Swashbuckle.AspNetCore 6.4.0

### Installation

To run the project, follow these steps:

#### 1. Clone the Repository
#### 2. Build Docker Images

Build Docker images using Docker Compose:
docker-compose build
#### 3. Run Docker Containers

Run the containers with the following command:

bash

docker-compose up

Accessing the Application

After the containers are up and running, your application will be accessible at the following addresses:
Swagger UI

Swagger UI is available at: http://localhost:8080/swagger
Endpoints
    http://localhost:8080/Tag/FetchAndSaveTags -POST
    http://localhost:8080/Tag/SortByTags - GET
    http://localhost:8080/Tag/RefreshTags - POST

#### 4. Stopping and Removing Containers

To stop and remove the containers, use the following:

bash

docker-compose down

Thank You.

