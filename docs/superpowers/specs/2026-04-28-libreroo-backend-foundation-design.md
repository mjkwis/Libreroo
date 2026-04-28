# Libreroo Phase 1 Backend Foundation Design

## Context

This design starts Libreroo as an interview-focused, backend-first implementation based on:

- `.ai/libreroo_concept.md`
- `.ai/docs/architecture.md`
- `.ai/docs/techstack.md`
- `.ai/agents/architect.md`

Phase 1 objective is a working backend quickly, while preserving clean architecture boundaries and a credible evolution path.

## Scope

### In scope

- Create `apps/libreroo-api` as the first runnable application.
- Use one ASP.NET Core Web API project with modular internal boundaries.
- Use PostgreSQL in Docker from day one.
- Implement foundational cross-cutting plumbing:
  - global exception handling
  - input validation baseline
  - Swagger/OpenAPI
  - health endpoint
- Establish core domain model and endpoint surface for:
  - Catalog (Books, Authors)
  - Members
  - Loans (borrow, return, active loans)

### Out of scope (phase 1)

- Authentication and authorization
- Frontend application implementation
- Microservices split, event bus, CQRS/event sourcing

## Architectural Approach

Recommended approach (approved): single API project with modular folders.

`apps/libreroo-api` structure:

- `Modules/Catalog/{Api,Application,Domain,Infrastructure}`
- `Modules/Members/{Api,Application,Domain,Infrastructure}`
- `Modules/Loans/{Api,Application,Domain,Infrastructure}`
- `Shared/{Api,Application,Domain,Infrastructure}` as needed for cross-cutting concerns

This keeps startup cost low while preserving explicit internal boundaries and dependency direction.

## Dependency Rules

Within each module:

1. `Domain` depends on nothing from other layers.
2. `Application` depends on `Domain` and abstractions.
3. `Infrastructure` depends on `Application` abstractions and `Domain`.
4. `Api` depends on `Application` and DTO contracts; never on persistence internals.

Cross-module interaction uses explicit interfaces/use cases only.

## Runtime and Data Design

### Runtime

- Root `docker-compose.yml` includes a `postgres` service for local development.
- API uses environment-based connection string, with development fallback in app settings.
- Swagger enabled in development profile.

### Persistence

- EF Core + Npgsql.
- One `DbContext` in phase 1.
- Entity configuration split by module to avoid boundary erosion.
- EF migrations stored in the API project.

### Initial entities and relationships

- `Author` 1:N `Book`
- `Book` 1:N `Loan`
- `Member` 1:N `Loan`
- `Loan.ReturnDate` nullable to represent active vs returned loans

## API Contract Baseline

- Books: `GET/POST/PUT/DELETE`
- Authors: full CRUD
- Members: full CRUD
- Loans:
  - `POST /loans` borrow
  - `POST /loans/{id}/return` return
  - `GET /loans/active` active loans

Controllers stay thin and delegate to application use cases/services.
DTOs are distinct from EF entities.

## Business Rules

Loan flow enforces:

- borrow rejected when `AvailableCopies <= 0`
- borrow decreases `AvailableCopies`
- return increases `AvailableCopies`
- returning an already returned loan is rejected

Rules are implemented in `Loans` domain/application layers, not controllers.

## Error Handling and Validation

- Global exception middleware maps known business/validation exceptions to consistent HTTP responses.
- Request validation uses FluentValidation or ASP.NET validation attributes (final pick in implementation plan).
- Validation failures return structured 4xx responses.

## Testing Strategy (Phase 1)

- Unit tests for loan business invariants.
- Integration tests for critical API flows (borrow and return).
- Smoke test for startup + health endpoint + DB connectivity.

Testing depth remains focused on highest-risk behavior first (loan invariants and transaction effects).

## ADR Updates

1. Confirm existing ADR alignment:
   - `.ai/docs/adr/0001-modular-monolith-with-separate-frontend.md`
2. Add new ADR:
   - `0002-no-authentication-in-phase-1.md`
   - Decision: defer authentication/authorization in phase 1.
   - Rationale: optimize for fast working MVP and domain logic demonstration.
   - Consequence: phase 2 must introduce auth with backward-compatible API evolution where possible.

## Milestone Acceptance Criteria

- `apps/libreroo-api` starts successfully.
- PostgreSQL container runs via Docker Compose.
- Initial migration can be applied.
- Swagger is reachable.
- `GET /health` returns OK.
- `GET /books` and `GET /loans/active` are callable.
- Borrow/return invariants are enforced by application/domain logic.

## Risks and Mitigations

- Risk: module boundaries degrade in a single project.
  - Mitigation: strict folder/layer conventions, code review checks, and explicit interfaces.
- Risk: auth deferral leaks into later phases.
  - Mitigation: ADR records explicit phase boundary and required phase 2 follow-up.
- Risk: time spent on architecture instead of runnable output.
  - Mitigation: start with minimal vertical slice and enforce scope control.
