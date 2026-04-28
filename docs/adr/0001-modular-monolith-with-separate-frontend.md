# 0001 - Modular Monolith Backend with Separate Frontend Package

- Date: 2026-04-28
- Status: accepted

## Context

Libreroo is an interview/demo project focused on demonstrating backend engineering quality in .NET while still
delivering a working fullstack application.  
The architecture should balance clarity, speed of implementation, and maintainability without introducing heavy
operational complexity.

## Decision

We will use:

- a modular monolith backend (`ASP.NET Core Web API`) with explicit module boundaries (`Catalog`, `Members`, `Loans`)
- a separate Angular frontend package in the same repository
- separate deploy units for API and web frontend
- PostgreSQL as a single relational datastore

## Consequences

Positive:

- Strong architectural signal for interviews without microservice overhead
- Clear domain boundaries and easier future extraction if needed
- Practical fullstack workflow in one repo with independent delivery pipelines

Trade-offs:

- Requires discipline to maintain module boundaries inside one codebase
- Separate deploy units add small release coordination overhead
- Shared DTO/contract evolution must be managed carefully

## Alternatives Considered

1. Classic layered monolith (`Controllers -> Services -> Repositories`)
    - Rejected as default because boundaries tend to blur as features grow.
2. Full Clean Architecture + broad CQRS
    - Rejected for current scope due to ceremony and slower delivery for a demo-focused project.
