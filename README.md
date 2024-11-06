# Smart Charging Microservices

This repository contains a set of microservices designed to manage an electric vehicle (EV) charging system. The services handle the creation, management, and validation of `Groups`, `Charge Stations`, and `Connectors`, ensuring all operations adhere to strict business rules.

## Table of Contents

- [Project Overview](#project-overview)
- [Prerequisites](#prerequisites)
- [Installation and Setup](#installation-and-setup)
- [Project Layout](#project-layout)
- [Business Rules and Constraints](#business-rules-and-constraints)
- [Service Details](#service-details)
- [Database](#database)
- [Event-Driven Communication](#eventdriven-communication)
- [API Documentation](#api-documentation)
- [Testing](#testing)


## Project Overview

The Smart Charging system is designed to manage a structured set of `Groups`, `Charge Stations`, and `Connectors` with the following key relationships and constraints:
- `Groups` contain `Charge Stations`, and `Charge Stations` contain `Connectors`.
- Each entity has strict validation rules and business constraints that must be followed.
- The system uses RabbitMQ for event-driven communication between services.

## Prerequisites

Before running the project, ensure that you have the following installed:

- .NET 6 SDK or later
- RabbitMQ (with management plugin enabled)
- Visual Studio or Visual Studio Code

## Installation and Setup
### RabbitMQ
1. Download and install [RabbitMQ](https://www.rabbitmq.com/download.html).
2. Start RabbitMQ server:
   ```bash
   rabbitmq-server 
   ```
3. Access the RabbitMQ Management Dashboard by navigating to http://localhost:15672/ in your browser. Use the default credentials (guest/guest) for logging in.
### Microservices
1. Clone the repository:
    ```bash
    git clone https://github.com/mouoent/smartcharging.git
    cd smartcharging
    ```
2. Restore NuGet packages:
    ```bash
    dotnet restore
    ```
3. Build the solution:
    ```bash
    dotnet build
    ```
4. Run each microservice
    ```bash
    dotnet run --project ./GroupService/GroupService.csproj
    dotnet run --project ./ChargeStationService/ChargeStationService.csproj
    dotnet run --project ./ConnectorService/ConnectorService.csproj
    ```

## Project Layout
The repository is divided into multiple microservices, each responsible for managing different parts of the system:

- **Infrastructure**: Contains shared clients, event listeners, and services.
- **GroupService**: Manages Groups and handles events related to Charge Stations and Connectors.
- **ChargeStationService**: Manages Charge Stations and interacts with Connectors and Groups.
- **ConnectorService**: Manages Connectors and communicates with Charge Stations.
- **Shared**: Contains shared data contracts, base models, event classes, interfaces and base classes.

        SmartCharging
        ├── Infrastructure
        │   ├── Clients
        │   ├── EventPublishers
        │   ├── Helpers
        │   ├── Services
        │   └── ServiceRegistration.cs
        ├── ChargeStationService
        │   ├── Controllers
        │   ├── Data
        │   ├── EventListeners
        │   ├── Interfaces
        │   ├── Models
        │   ├── Repositories
        │   ├── Services
        │   ├── appsettings.json
        │   └── Program.cs
        ├── ConnectorService
        │   ├── Controllers
        │   ├── Data
        │   ├── EventListeners
        │   ├── Interfaces
        │   ├── Models
        │   ├── Repositories
        │   ├── Services
        │   ├── appsettings.json
        │   └── Program.cs
        ├── GroupService
        │   ├── Controllers
        │   ├── Data
        │   ├── EventListeners
        │   ├── Interfaces
        │   ├── Models
        │   ├── Repositories
        │   ├── Services
        │   ├── appsettings.json
        │   └── Program.cs
        └── Shared
            ├── Events
            ├── Interfaces
            ├── Models
            ├── Repositories
            └── Services

## Business Rules and Constraints
- #### Group:
    - Must have a unique `Identifier`, `Name`, and `Capacity in Amps`.
    - Can contain multiple `Charge Stations`.
    - Deletion Rule: If a `Group` is deleted, all associated `Charge Stations` and their `Connectors` are also deleted.
- #### Charge Station:
    - Must have a `unique Identifier`, `Name`, and `Connectors` (between 1 and 5).
    - Cannot exist without a `Group`.
- #### Connector:
    - Must have an `integer Identifier` unique within a `Charge Station` and a `Max current in Amps`.
    - Cannot exist without a `Charge Station`
    - A `Connector` cannot exist without a Charge Station.
    - The `Max current in Amps` of a Connector can be updated.

### Functional Requirements Implemented:
- `Groups`, `Charge Stations`, and `Connectors` can be created, updated, removed, and retrieved.
- Removing a `Group` deletes all associated `Charge Stations`.
- A `Charge Station` can only be in one `Group` at a time.
- `Connectors` are uniquely identified within their `Charge Station`.
- The Max current in Amps of a `Connector` can be changed.
- The Capacity in Amps of a `Group` should be greater than or equal to the sum of the Max current in Amps of all its `Connectors.`
- All operations validate the rules above and reject any request that would violate them.

## Service Details
### Group Service
- **Description**: Handles group-related operations and includes event listeners for handling charge station and connector events.
- **Event Listeners**:  Listeners for `ConnectorCreatedEvent`, `ChargeStationUpdatedEvent`, etc.
- **Models**: Group models and DTOs.
- **Repositories**: Implements the `GroupRepository`.
- **Program.cs**: Sets up service dependencies, database seeding, and hosted event listeners.
### ChargeStation Service
- **Description**: Manages charge stations and their interactions with connectors and groups.
- **Event Listeners**:  Listeners for `GroupDeletedEvent`, `ConnectorCreatedEvent`, etc.
- **Models**: ChargeStation models and DTOs.
- **Repositories**: Implements the `ChargeStationRepository`.
- **Program.cs**: Sets up service dependencies, database seeding, and hosted event listeners.
### Connector Service
- **Description**: Responsible for connector operations, ensuring they align with the capacities of associated charge stations and groups.
- **Event Listeners**:  Listeners for `ChargeStationDeletedEvent`, etc.
- **Models**: Connector models and DTOs.
- **Repositories**: Implements the `ConnectorRepository`.
- **Program.cs**: Sets up service dependencies, database seeding, and hosted event listeners.

## Database
Each service utilizes an in-memory database for data storage. These databases are seeded from CSV files located in their respective Data directories. The structure of the CSV files should match the corresponding entity models.

- **Group Service**: Groups.csv
- **Charge Station Service**: ChargeStations.csv
- **Connector Service**: Connectors.csv
### CSV File Structure:
Ensure the data in each CSV matches the structure defined by the models (e.g., Group, Charge Station, Connector).

## Event-Driven Communication
RabbitMQ is used for event-driven communication between the microservices. Each service subscribes to and processes events based on its business logic.

- Exchange: SmartCharging
- Type: direct
- Routing Keys: Events are routed using the Name of the event (e.g., ConnectorCreatedEvent).

### Event Listeners
Each service has its own set of listeners that process relevant events to ensure data consistency and maintain business rules.

## API Documentation
Each microservice includes Swagger for API documentation. You can access the Swagger UI for each service at:
- **Group Service**: http://localhost:5001/
- **ChargeStation Service**: http://localhost:5002/
- **Connector Service**: http://localhost:5003/

### Testing
The solution includes unit tests for each service, ensuring core business logic and validations are covered. Tests are located in the `SmartCharging.Tests/UnitTests` directory.

```bash
dotnet test
```
