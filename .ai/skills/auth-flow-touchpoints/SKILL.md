---
name: auth-flow-touchpoints
description: Change JWT/OAuth and token lifecycle behavior safely in CXJ auth boundaries. Use when touching /api/jwtauth endpoints, refresh/access token logic, auth configuration, and backward compatibility touchpoints.
---

# Skill: Auth Flow Touchpoints

## Purpose

Change authentication/token flows safely while preserving existing JWT/OAuth behavior and compatibility expectations.

## Trigger Conditions

Use this skill when changing:

- Endpoints under `/api/jwtauth`
- Refresh/access token generation/validation behavior
- Dialog OAuth exchange, encryption/decryption, or auth config wiring

## Pattern Anchors (use as references)

- Auth controller boundary:
  `dialog-authentication/src/main/java/com/roche/dia/dto/cex/auth/controller/JWTController.java`
- OAuth client behavior: `dialog-authentication/src/main/java/com/roche/dia/dto/cex/auth/service/DialogOAuthClient.java`
- Auth config wiring: `dialog-authentication/src/main/java/com/roche/dia/dto/cex/auth/config/DialogConfig.java`
- API-side auth integration touchpoint:
  `felix-api/src/main/java/com/roche/dia/dto/cex/felix/authorization/DialogTokenAuthenticator.java`

## Non-Negotiable Invariants

- Auth domain logic remains in `dialog-authentication`.
- Token validation precedes token mint/refresh actions.
- Security-sensitive config values come from existing config sources only.
- Backward compatibility for legacy endpoints is preserved unless explicitly removed with rollout plan.

## Workflow

1. Map current endpoint and caller dependencies before changing behavior.
2. Keep controller logic thin and delegate to token/auth services.
3. Preserve validation and unauthorized response semantics.
4. Keep encryption/decryption/error handling explicit and auditable.
5. Add regression coverage for valid token, invalid token, and provider error paths.
6. Document any deprecation/removal timeline for compatibility endpoints.

## Review Checklist

- Response codes for invalid/expired token flows remain intentional.
- No sensitive token/secret value is logged.
- Legacy compatibility endpoints are either preserved or intentionally sunset.
- Redirect URI/client ID flow remains config-driven and validated.
- Error mapping to domain exceptions remains stable.

## Anti-Patterns to Reject

- Bypassing token validity checks before issuing new tokens.
- Mixing auth-service concerns into unrelated modules.
- Silent behavior changes in `/api/jwtauth` that break existing clients.

## Verification Commands

- `./gradlew :dialog-authentication:test`
- `./gradlew :felix-api:test`
- `./gradlew :felix-core:test` (if downstream authorization logic is impacted)

## Done Criteria

- Token lifecycle behavior is secure and backward-safe.
- Auth endpoints and services are covered by targeted tests.
- Any compatibility-impacting change is explicitly documented.

