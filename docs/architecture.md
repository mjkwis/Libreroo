# Libreroo Architecture

## Scope and Intent

Libreroo is an interview/demo-focused project.  
Primary goal: show solid backend architecture and clean fullstack integration without overengineering.

## Architecture Decision Summary

- Backend: modular monolith in one ASP.NET Core API application
- Frontend: Angular application in the same repository, separate package and deploy unit
- Deployment model: two deployable units (`libreroo-api`, `libreroo-web`) plus PostgreSQL

## Repository Structure

```text
Libreroo/
  apps/
    libreroo-api/
    libreroo-web/
  .ai/
    docs/
      architecture.md
      adr/
        README.md
        0001-modular-monolith-with-separate-frontend.md
```

## Backend Structure (Modular Monolith)

Modules:

- `Catalog` (Books, Authors)
- `Members`
- `Loans`

Each module should contain:

- `Api` (controllers/endpoints)
- `Application` (use cases, orchestration)
- `Domain` (entities, domain rules)
- `Infrastructure` (EF Core persistence, mapping, repository implementations)

Shared cross-cutting code:

- global exception handling
- validation plumbing
- common result/error primitives
- logging and observability hooks

## Dependency Rules

1. `Domain` has no dependency on `Application`, `Api`, or `Infrastructure`.
2. `Application` depends on `Domain` and abstractions only.
3. `Infrastructure` depends on `Application` abstractions and `Domain`.
4. `Api` depends on `Application` (and DTO contracts), never directly on persistence details.
5. Cross-module calls go through explicit interfaces/use cases, not direct internal class access.

## Frontend and API Contract

- Angular app consumes API over HTTP.
- Keep backend DTOs and frontend models aligned through:
    - OpenAPI-first contract generation (preferred), or
    - manually versioned DTO contracts with strict review.
- Do not couple frontend directly to database concerns.

## Data and Transaction Boundaries

- Single PostgreSQL database for the monolith.
- One EF Core `DbContext` can be used initially, with clear module entity configuration boundaries.
- Business invariants (for example loan/return rules) live in domain/application logic, not in controllers.

## Non-Goals (Current Scope)

- No microservices split
- No distributed messaging/event bus
- No advanced CQRS/event sourcing
- No premature multi-database segregation

## Evolution Triggers

Revisit architecture only when one of these appears:

- module change velocity causes frequent merge conflicts
- module ownership differs across teams
- scaling or reliability needs force independent deployments
- bounded context complexity outgrows current in-process boundaries

Related decisions are tracked in ADRs under `.ai/docs/adr/`.
