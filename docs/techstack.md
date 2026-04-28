# Libreroo Tech Stack

## Core Stack

- Backend: `.NET 10 SDK` + `ASP.NET Core Web API`
- Frontend: `Angular 20` + `TypeScript`
- Database: `PostgreSQL`

## Backend Technologies

- `Entity Framework Core` for ORM and migrations
- `Npgsql` provider for PostgreSQL integration
- `Swagger / OpenAPI` for API documentation
- `FluentValidation` (or built-in validation attributes) for input validation
- Layered architecture modules:
    - `Libreroo.Api`
    - `Libreroo.Application`
    - `Libreroo.Domain`
    - `Libreroo.Infrastructure`

## Frontend Technologies

- `Angular Router` for navigation
- `Angular HttpClient` for API communication
- `Reactive Forms` for add/edit flows
- `RxJS` for state and async handling

## Cross-Cutting

- Global exception handling on API side
- DTO-based request/response contracts
- Optional containerization with `Docker` (later phase)

## Why This Stack

This stack keeps the project focused on clean backend architecture and business logic while using a modern, stable
frontend and a production-grade relational database.
