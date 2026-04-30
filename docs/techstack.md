# Libreroo Tech Stack

## Core Stack

- Backend: `.NET 10 SDK` + `ASP.NET Core Web API`
- Frontend: `Angular 20` + `TypeScript`
- Database: `PostgreSQL`
- Identity Provider: `Keycloak` (OIDC/OAuth2)

## Backend Technologies

- `Entity Framework Core` for ORM and migrations
- `Npgsql` provider for PostgreSQL integration
- `Swagger / OpenAPI` for API documentation
- `FluentValidation` (or built-in validation attributes) for input validation
- `Microsoft.AspNetCore.Authentication.JwtBearer` for API token validation
- `ASP.NET Core Authorization Policies` for role/permission checks
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
- OIDC Authorization Code + PKCE flow against Keycloak
- Role-aware route guards and HTTP auth interceptors

## Cross-Cutting

- Global exception handling on API side
- DTO-based request/response contracts
- Local containerization with `Docker Compose` for PostgreSQL and Keycloak
- Hybrid authorization model:
    - Keycloak handles authentication and token issuance
    - Libreroo persists role/permission mapping and domain user links in PostgreSQL

## Why This Stack

This stack keeps the project focused on clean backend architecture and business logic while using a modern, stable
frontend and a production-grade relational database.  
Keycloak accelerates secure authN rollout in Phase 2, while PostgreSQL-backed authorization data keeps domain-specific
access control inside Libreroo.
