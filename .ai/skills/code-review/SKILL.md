---
name: code-review
description: Review newly introduced functionality with strict focus on correctness, architecture boundaries, clean code principles (SOLID, KISS, DRY, ACID), security, and testing quality. Use when reviewing PRs or branches with new behavior.
---

# Skill: Code Review

## Purpose

Use this skill when reviewing newly introduced functionality from another developer in this repository.

Primary goals:

- Keep code clean, simple, and maintainable.
- Enforce SOLID, KISS, DRY, and project best practices.
- Validate data consistency and transaction safety expectations (ACID where applicable).
- Prevent architecture boundary violations across CXJ modules.

## Trigger Conditions

Use this skill when any of the following is true:

- A PR/branch introduces new features or extends existing behavior.
- A change adds new endpoints, services, repositories, integrations, or DTOs.
- A change touches critical flows across `felix-api`, `felix-core`, `felix-model`, `felix-mulesoft-integration`,
  `dialog-authentication`, or `felix-universal`.

For frontend tasks, execute review as the final phase via:

- `frontend-delivery-flow` (implement -> test -> review)

Do not use this as the primary mode for pure formatting-only or non-functional changes.

## Required Context

Before reviewing, read:

- `AGENTS.md`
- `.ai/libreroo_concept.md`
- `.ai/rules/backend.md`
- `.ai/rules/testing.md`
- `.ai/rules/integrations.md`
- `.ai/rules/frontend.md` (if frontend is touched)

## Review Priorities (in order)

1. Correctness and regressions
2. Security and data handling
3. Architecture/layer boundary compliance
4. Readability and maintainability
5. Performance and operational resilience
6. Test coverage and verification quality

## Project-Specific Architecture Guardrails

- Controllers stay thin in `felix-api`; business logic belongs in `felix-core`.
- MongoDB access belongs in `felix-core` repositories/query implementations.
- External API clients belong in `felix-mulesoft-integration`.
- Auth/JWT/OAuth concerns belong in `dialog-authentication`.
- Shared contracts belong in `felix-model`; shared helpers in `felix-utils`.
- Avoid duplicate models/utilities if equivalent artifacts already exist.

## Review Checklist

### A) Clean Code and Simplicity

- Names are intention-revealing and consistent with existing module language.
- Methods/classes are focused and not overloaded with multiple responsibilities.
- Control flow is easy to follow; avoid unnecessary nesting and branching.
- Changes prefer minimal complexity (KISS) over speculative abstractions.

### B) SOLID

- Single Responsibility: each class/component has one clear reason to change.
- Open/Closed: extension is favored over modification when practical.
- Liskov Substitution: inherited/implemented contracts remain behaviorally compatible.
- Interface Segregation: consumers are not forced to depend on unused methods.
- Dependency Inversion: high-level policy is not tightly coupled to low-level detail.

### C) DRY

- No repeated business logic across controllers/services/components.
- Reuse existing mapping/utility/domain patterns before introducing new helpers.
- Avoid parallel implementations of similar repository or integration logic.

### D) ACID and Data Consistency (where applicable)

- Multi-step persistence flows have clear consistency boundaries.
- Transactional behavior is explicit where multi-entity atomicity is required.
- Failure paths do not leave partial, invalid, or contradictory persisted state.
- Idempotency/retry behavior is considered for integration + persistence workflows.

### E) API and Contract Stability

- Public endpoint behavior remains backward compatible unless intentionally changed.
- Validation and error responses are consistent with existing conventions.
- DTO changes are coordinated between backend and frontend consumers.

### F) Security and Integration Safety

- No secrets, tokens, or credential material introduced in code/config.
- Existing auth/correlation/timeout/error-handling patterns are preserved.
- External provider payloads remain isolated from public API contracts unless intentional.

### G) Testing Quality

- New functionality has tests at the lowest effective level.
- High-risk paths include integration/e2e validation as needed.
- Bug fixes include regression tests where practical.
- Review notes include what was run, what was skipped, and residual risk.

## Findings Format (mandatory)

Report findings first, ordered by severity:

- `High`: correctness, data-loss, security, contract break, major regression risk.
- `Medium`: maintainability, architecture drift, incomplete testing, performance concerns.
- `Low`: readability, minor refactor opportunities, non-blocking cleanup.

For each finding include:

- Severity
- File path and line reference
- What is wrong
- Why it matters
- Concrete fix recommendation

If no findings:

- State explicitly that no findings were found.
- Mention residual risks/test gaps if any validation was not executed.

## Suggested Review Flow

1. Identify touched modules and map them to ownership boundaries.
2. Scan for architecture violations first.
3. Review correctness and error paths.
4. Check SOLID/KISS/DRY/ACID implications.
5. Validate tests and missing coverage.
6. Publish findings in severity order with exact file references.

## Loop Guard

If the same unresolved review question repeats twice without progress, stop and ask the code owner for a decision.

