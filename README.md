# CookinSocial.API

Production-style social cooking platform built with ASP.NET Core and modern backend development practices.

This project is being developed as a showcase of real-world backend architecture and engineering patterns commonly used in production applications.

## Vision

CookinSocial enables users to discover recipes, share cooking experiences, connect with other cooks, and organize meal planning through a social-first platform.

The project focuses on building a maintainable and scalable backend using a layered architecture approach, with clear separation of concerns between API, business logic, domain models, and infrastructure components. Some naming conventions etc. are inspired by clean architecture but it is not strictly followed in this project.

## Planned Features

### Authentication & Accounts

* User registration and login
* JWT authentication
* Email verification
* Password reset
* Profile management

### Recipes

* Create and manage recipes
* Recipe feeds
* Search and filtering
* Categories and cuisines
* Reviews and ratings

### Social Features

* Follow users
* Save recipes
* Like recipes
* User interactions and activity feeds

### Shopping Lists

* Personal shopping lists
* Ingredient management

### Notifications

* Push notifications
* Email notifications

### Background Processing

* Scheduled jobs
* Media processing workflows

## Architecture

The solution follows a layered architecture approach and incorporates commonly used enterprise application patterns, including Repository and Unit of Work.

```text
CookinSocial.API
        │
        ▼
CookinSocial.Application
        │
        ▼
CookinSocial.Infrastructure
        │
        ▼
CookinSocial.Domain
```

## Technology Stack

* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* ASP.NET Core Identity
* JWT Authentication
* Repository Pattern
* Unit of Work Pattern
* Swagger / OpenAPI
* Docker
* GitHub Actions

## Project Status

🚧 Early Development

The project is being built publicly with a focus on maintainability, scalability, and production-ready development practices.
