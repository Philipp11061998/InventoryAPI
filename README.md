# Inventory Management API

A modern REST API for managing products, warehouses, stock movements, and aggregated inventory levels.

This project was built as a backend portfolio project to demonstrate practical API design, business logic, database integration, and clean project structure with .NET 8, Entity Framework Core, SQL Server, and Docker.

## Overview

The Inventory Management API models a realistic warehouse and stock management workflow.

Instead of storing a static stock value per product, the current inventory is calculated from historical stock movements. This makes the system more transparent, extensible, and closer to real business scenarios.

The project currently supports:
- product management
- warehouse management
- stock movements (`Inbound` / `Outbound`)
- aggregated inventory queries
- soft delete for products
- validation of business rules such as preventing negative stock

## Tech Stack

- **.NET 8**
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **SQL Server**
- **Docker**
- **Swagger / OpenAPI**
- **C#**

## Architecture

The project follows a simple layered structure:

- **Controllers** handle HTTP requests and responses
- **Services** contain business logic
- **Data** contains the EF Core `DbContext`
- **DTOs** are used for request models
- **Models** represent the internal domain and database entities
- **ResponseDTOs** Responses aren't database entities and ge mapped by servicescoped functions

Current flow:

`Controller -> Service -> DbContext -> SQL Server`

This structure keeps the code readable, testable, and easy to extend.

## Core Concepts

### Products
Products can be created, updated, queried, and soft deleted.

### Warehouses
Warehouses represent physical storage locations.

### Inventory Movements
Inventory is not stored as a fixed value.  
Instead, each stock change is recorded as a movement:

- `Inbound` increases stock
- `Outbound` decreases stock

### Inventory Aggregation
The current stock level is calculated by grouping movements by product and warehouse and summing all signed amounts.

## Features

### Product API
- `GET /api/products`
- `GET /api/products/{id}`
- `POST /api/products`
- `PUT /api/products/{id}`
- `DELETE /api/products/{id}`

### Warehouse API
- `GET /api/warehouses`
- `GET /api/warehouses/{id}`

### Movement API
- `GET /api/movements`
- `GET /api/movements/{id}`
- `POST /api/movements`

### Inventory API
- `GET /api/inventory`
- optional filtering via query parameters:
  - `productId`
  - `warehouseId`

Example:

`GET /api/inventory?productId=1&warehouseId=2`

## Business Rules

The API already enforces several important business rules:

- products must exist before movements can be created
- warehouses must exist before movements can be created
- inactive products cannot be used for new stock movements
- inactive warehouses cannot be used for new stock movements
- movement amount must be greater than zero
- outbound movements are rejected if the available stock is insufficient
- products are soft deleted instead of physically removed

## Why stock is calculated from movements

A central design decision in this project is that stock is derived from movement history instead of being stored directly.

Advantages:
- better auditability
- easier debugging of stock changes
- more realistic business modeling
- extensibility for future features like transfers, alerts, and reporting

## Validation

Validation is handled with DTOs and data annotations.

Examples:
- required request fields
- string length constraints
- numeric range checks

## Database

The project uses SQL Server running in Docker.

An initialization script is used to:
- create the database if needed
- create tables
- seed demo data for products, warehouses, and movements

## Local Setup

### Prerequisites
- .NET 8 SDK
- Docker
- SQL Server container running locally

### Run the project
1. Start the SQL Server container
2. Ensure the database init script has run successfully
3. Start the API
4. Open Swagger

Swagger is available at:

`http://localhost:8080/swagger`

## Example API Workflow

### 1. Create a product
Create a new product via `POST /api/products`

### 2. Create a warehouse
Create or use an existing warehouse

### 3. Add stock
Create an `Inbound` movement for a product and warehouse

### 4. Remove stock
Create an `Outbound` movement

### 5. Query current inventory
Use `GET /api/inventory` to view calculated stock levels

## Project Goals

This project was created to demonstrate backend development skills relevant to real-world applications, including:

- REST API design
- service-based architecture
- asynchronous database access
- business rule validation
- relational database modeling
- LINQ and aggregation queries
- Docker-based local development

## Current Limitations

This is an actively evolving portfolio project.

Planned improvements include:
- response DTOs
- global error handling middleware
- automated tests
- authentication and authorization
- logging
- richer warehouse endpoints
- transfer workflows between warehouses
- low-stock monitoring

## Roadmap

### Next planned improvements
- ~~introduce response DTOs ~~ (finished 22.03.2026)
- add centralized exception handling
- add automated tests for business logic
- improve API consistency and error responses
- extend warehouse CRUD functionality
- add authentication and authorization
- improve documentation and setup experience

## Authentication Roadmap

Authentication is planned for a future version of the project.

The long-term goal is to add modern authentication and authorization concepts, potentially with cloud-based identity integration later on. For now, the focus is on building a solid backend foundation with correct business logic, data modeling, and API design.

## Portfolio Context

This project is part of my backend portfolio and was built to deepen my experience with:
- backend architecture
- .NET API development
- SQL-based data modeling
- service-oriented design
- realistic business logic implementation

## Author

Backend portfolio project by Philipp.