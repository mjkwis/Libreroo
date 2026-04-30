# Libreroo Phase 2 Auth and Authorization Design

## Context

This spec defines Phase 2 security scope based on:

- conversation decisions on 2026-04-29
- `member`, `librarian`, and `admin` role requirement
- ADR `0003-phase-2-auth-with-keycloak-and-postgresql-authorization.md`
- updated `docs/architecture.md` and `docs/techstack.md`

Phase 1 intentionally shipped without auth (ADR 0002). Phase 2 introduces secure identity and role-based behavior
without breaking module boundaries.

## Scope

### In scope

- Add Keycloak to local Docker runtime for OIDC/OAuth2 authentication.
- Add API JWT bearer validation against Keycloak realm metadata.
- Add `Access` module in backend for local authorization data in PostgreSQL:
  - app user identity link (`keycloak subject -> local user`)
  - role assignments (`member`, `librarian`, `admin`)
  - optional permission rows for future fine-grained access
- Enforce endpoint authorization policies using JWT identity (`sub`) plus **DB-authoritative** role/permission data.
- Replace Phase 1 frontend "selected member from localStorage" identity model with authenticated session.
- Add login/logout and role-aware route/page behavior in Angular app.
- Add tests for authN/authZ enforcement and role-based behavior.

### Out of scope

- SSO with third-party IdPs beyond Keycloak local realm setup
- multi-tenant realm model
- external user self-registration workflows
- audit/event streaming and SIEM integration

## Approach Selection

Chosen approach: **Hybrid auth model (Keycloak for authN, PostgreSQL for authZ)**.

Alternatives considered:

1. Keycloak-only roles/authorization
2. full custom JWT/auth system in Libreroo
3. postpone auth to later phases

Why this approach:

- aligns with ADR 0003
- keeps security-critical authentication in standard infrastructure
- keeps domain-specific authorization ownership inside the monolith

## Architecture

### Runtime components

- `libreroo-api` (ASP.NET Core API)
- `libreroo-web` (Angular SPA)
- `postgres` (Libreroo DB)
- `keycloak` (OIDC/OAuth2 provider)

### Backend module boundaries

- New module: `Modules/Access/{Api,Application,Domain,Infrastructure}`
- Existing modules (`Catalog`, `Members`, `Loans`) consume access decisions through abstractions, not direct DB queries.
- `Api` layer only applies policies and delegates to application services.

### Identity and authorization model

- JWT `sub` from Keycloak is the primary external identity key.
- `Access` module maps `sub` to local user record and role assignments in PostgreSQL.
- API authorization **must not** trust role claims from access tokens as the source of truth.
  Token claims can be logged/inspected for diagnostics, but effective app role checks are resolved from `Access` data.
- API request authorization flow:
  1. Validate token (issuer, audience, signature, lifetime).
  2. Resolve local user + roles by `sub` from PostgreSQL.
  3. Apply endpoint policy/permission checks.

## Role Model and Endpoint Intent

- `member`
  - browse catalog
  - borrow books for self
  - return own loans
  - list own active loans
- `librarian`
  - all member capabilities
  - create/manage members
  - view all active loans
  - perform loan return operations for members
- `admin`
  - all librarian capabilities
  - manage role assignments and access mappings

## API Contract Evolution

Phase 2 adds or changes behavior:

- New endpoint: `GET /loans/me/active`
- Existing `GET /loans/active` becomes `librarian|admin` scope
- Existing `POST /loans` behavior:
  - `member`: member id inferred from authenticated user linkage (`AccessUser -> Member`)
  - `librarian|admin`: can submit explicit member id for on-behalf operations
- Existing `POST /loans/{id}/return` behavior:
  - `member`: can return only own loan
  - `librarian|admin`: can return loans for any member
- New access management endpoints under `/access/users` for admin operations

## Frontend Behavior

- Add OIDC login/logout flow (Authorization Code + PKCE).
- Remove localStorage-selected-member as authentication source.
- Add role-aware navigation and route guards:
  - `catalog`: authenticated users
  - `loans`: authenticated users with role-specific views
  - `member`: `librarian|admin`
  - `access` (new): `admin`
- Use authenticated identity context in API calls and UI state.

## Data Model (Access Module)

Core entities:

- `AccessUser` (`Id`, `Subject`, `Email`, `DisplayName`, `IsActive`)
- `AccessRoleAssignment` (`UserId`, `Role`, `GrantedAtUtc`)
- `AccessPermission` (optional phase-2 scaffold)

Relationships:

- one `AccessUser` to many `AccessRoleAssignment`
- optional linkage from `AccessUser` to `Members.Member` for member role behavior

Bootstrap and lifecycle:

- Seed one initial admin mapping in local/dev environments (subject/email provided via configuration).
- First successful login for unknown `sub` may create a minimal `AccessUser` profile, but with **no roles** by default.
- Role grants and member linkage are admin-managed operations.

## Provisioning and Bootstrap Strategy

- Provisioning source:
  - identity proof from Keycloak token (`sub`, email, display hints)
  - authorization state from Libreroo `Access` module
- First-admin bootstrap:
  - deterministic startup seed from config/environment in local/dev
  - production bootstrap executed explicitly (migration/manual seed runbook), not implicit auto-grant
- Missing access mapping behavior:
  - authenticated but unprovisioned users receive `403 Forbidden` for protected app flows
  - response should be generic (no sensitive internals)

## Error Handling and Security Constraints

- Unauthenticated requests return `401`.
- Unauthorized requests return `403`.
- Do not expose token internals or secrets in response/logs.
- Preserve global exception contract; extend with access-related domain exceptions where needed.
- Environment rules:
  - `RequireHttpsMetadata=false` allowed only in local development profiles.
  - admin bootstrap credentials/data are configuration-driven and never hardcoded in source.

## Testing Strategy

Backend:

- unit tests for access role resolution and policy checks
- integration tests for `401/403` matrices by endpoint/role
- integration tests for member self-scope behavior (`/loans/me/active`, self-return)
- integration test path that validates real Keycloak token handling (issuer/audience/signature),
  in addition to fast test-auth-handler coverage

Frontend:

- unit tests for auth service, route guards, and auth interceptor
- component tests for role-conditioned rendering/navigation
- smoke test for login callback routing and authenticated app shell

## Acceptance Criteria

- Keycloak runs locally via Docker with realm/client bootstrap docs.
- API accepts valid Keycloak JWT tokens and rejects invalid/missing tokens.
- Role policies are enforced for member/librarian/admin scenarios.
- Access mappings and role assignments persist in PostgreSQL.
- Frontend no longer depends on manual member selection for identity.
- All touched backend/frontend tests pass in local CI command set.

## Risks and Mitigations

- Risk: role drift between Keycloak and local DB.
  - Mitigation: make PostgreSQL mapping authoritative for API authorization and add admin management endpoints.
- Risk: phase transition breaks existing frontend flows.
  - Mitigation: keep route surface stable where possible and add explicit login/session UX.
- Risk: over-broad policy wiring leaks privileged access.
  - Mitigation: endpoint-by-endpoint authorization test matrix and default-deny policy baseline.
- Risk: first-admin lockout or inconsistent bootstrap across environments.
  - Mitigation: explicit bootstrap strategy, documented runbook, and startup validation for bootstrap config.

## Implementation Handoff

Recommended execution order:

1. infrastructure and auth wiring (`docker-compose`, JWT config)
2. access module and persistence
3. API policy enforcement and contract updates
4. frontend auth/session/guards
5. verification + docs updates
