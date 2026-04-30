# 0003 - Phase 2 Auth with Keycloak and PostgreSQL Authorization

- Date: 2026-04-29
- Status: accepted

## Context

Phase 1 intentionally shipped without authentication (ADR 0002).  
Phase 2 requires secure access control for three roles: `member`, `librarian`, and `admin`.

The project needs:

- standards-based authentication without building a custom IdP
- domain-aware authorization controlled by Libreroo itself
- compatibility with the existing modular monolith and PostgreSQL data model

## Decision

Use a hybrid model:

1. **Authentication (authN):** Keycloak issues and manages OIDC/OAuth2 tokens.
2. **Authorization (authZ):** Libreroo stores local user-role/permission state in PostgreSQL.
3. **Runtime enforcement:** API validates JWTs, resolves local user/access records, and enforces endpoint policies for
   `member`, `librarian`, and `admin`.

Boundary ownership:

- Keycloak owns identity proof, login flow, session lifecycle, and token issuance.
- Libreroo owns domain linkage (for example user-to-member mapping), app-specific roles, and fine-grained permissions.

## Consequences

Positive:

- Faster and safer auth rollout using a proven IdP.
- Keeps domain authorization logic and data in the monolith where business rules already live.
- Supports future expansion from coarse roles to finer permissions without replacing identity architecture.

Trade-offs:

- Additional operational component (Keycloak) in local/dev and deployment environments.
- Authorization now depends on both token claims and database state, increasing integration complexity.
- Requires clear migration and synchronization strategy for user lifecycle between Keycloak and Libreroo records.

## Alternatives Considered

1. **Roles only in Keycloak, no local authorization data**
    - Rejected because domain linkage and app-specific permission evolution become harder to manage.
2. **Fully custom JWT/auth implementation in Libreroo**
    - Rejected due to higher security risk and slower delivery compared to a standard IdP.
3. **Defer auth further beyond Phase 2**
    - Rejected because Phase 2 explicitly requires protected role-based flows.
