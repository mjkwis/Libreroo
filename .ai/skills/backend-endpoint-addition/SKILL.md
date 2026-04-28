---
name: backend-endpoint-addition
description: Implement or extend backend endpoints in CXJ with correct layering across felix-api and felix-core. Use when adding new REST endpoints, service orchestration, validation, and backend tests.
---

# Skill: Backend Endpoint Addition

## Purpose

Add or extend backend functionality while preserving established CXJ layering and style.

## Trigger Conditions

Use this skill when a change introduces:

- A new REST endpoint in `felix-api`
- New orchestration/business logic in `felix-core`
- New or updated request/response contracts used by backend consumers

## Pattern Anchors (use as references)

- Thin controller style: `felix-api/src/main/java/com/roche/dia/dto/cex/felix/cases/CaseController.java`
- Controller with auth + validation + delegation:
  `felix-api/src/main/java/com/roche/dia/dto/cex/felix/issues/InstrumentIssueController.java`
- Shared boundary exception handling:
  `felix-api/src/main/java/com/roche/dia/dto/cex/felix/common/CustomExceptionHandler.java`
- Service orchestration in core:
  `felix-core/src/main/java/com/roche/dia/dto/cex/felix/issues/InstrumentIssueService.java`

## Non-Negotiable Invariants

- Controllers stay thin and delegate.
- Business logic and data orchestration stay in `felix-core`.
- MongoDB access stays in `felix-core` repositories/custom repositories.
- External API calls stay out of controllers.
- Backward compatibility is preserved unless intentionally changed and documented.

## Workflow

1. Confirm module ownership and API contract impact.
2. Implement or extend service behavior in `felix-core` first.
3. Add/update endpoint in `felix-api` as a boundary adapter only.
4. Apply input validation and reuse existing exception/error-code patterns.
5. Reuse existing DTO/entities/builders before adding new models.
6. Add tests for service and endpoint behavior.

## Review Checklist

- Endpoint path, method, and response shape match existing conventions.
- `@PreAuthorize` and auth resolution patterns align with nearby controllers.
- No repository or integration client usage directly inside controller classes.
- Service method names and responsibilities are single-purpose and readable.
- Error flows map to existing exception types and handler behavior.
- New behavior has test coverage at the lowest useful level.

## Anti-Patterns to Reject

- DB reads/writes in `felix-api`.
- Business branching inside controller methods.
- Duplicating existing domain model classes in feature-local packages.
- New ad-hoc error response shapes that bypass existing handling.

## Verification Commands

- `./gradlew :felix-core:test`
- `./gradlew :felix-api:test`
- `./gradlew :felix-core:integrationTest` (when behavior touches integration/persistence boundaries)

## Done Criteria

- Correct module placement and preserved dependency direction.
- Tests pass for touched backend scope.
- Contract or behavior changes are documented in PR notes.

