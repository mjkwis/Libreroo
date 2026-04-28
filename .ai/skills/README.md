# Skills Catalog (`.ai/skills`)

This folder contains project-specific skills for implementing and reviewing changes while preserving existing CXJ
patterns.

## Available skills

- `code-review/SKILL.md`
    - Use for reviewing newly introduced functionality.
    - Focus: clean code, SOLID, KISS, DRY, ACID, security, compatibility, test quality.

- `code-sense-check/SKILL.md`
    - Use to sanity-check whether produced code truly makes sense in real scenarios.
    - Focus: behavior correctness, edge-case validity, side-effect safety, boundary fit.

- `backend-endpoint-addition/SKILL.md`
    - Use when adding/changing backend endpoints across `felix-api` + `felix-core`.
    - Focus: thin controllers, service orchestration, boundary-safe API changes.

- `integration-client-change/SKILL.md`
    - Use when changing external clients in `felix-mulesoft-integration` or auth remote calls in
      `dialog-authentication`.
    - Focus: correlation IDs, error mapping, credential/config discipline.

- `mongo-query-patterns/SKILL.md`
    - Use when changing Mongo repositories/custom queries in `felix-core`.
    - Focus: query/update correctness, scope filtering, consistent not-found behavior.

- `dto-contract-evolution/SKILL.md`
    - Use when backend/frontend contracts change.
    - Focus: `felix-core` TypeScript generation and `felix-universal` compatibility.

- `auth-flow-touchpoints/SKILL.md`
    - Use when touching JWT/OAuth/token refresh behavior.
    - Focus: secure token lifecycle and backward-safe endpoint behavior.

- `regression-test-selection/SKILL.md`
    - Use to choose risk-based test scope for PR validation.
    - Focus: minimal but sufficient coverage across unit/integration/e2e suites.

- `frontend-unit-testing/SKILL.md`
    - Use to design and implement frontend unit tests in `felix-universal`.
    - Focus: deterministic Angular tests, stable mocking, behavior-first assertions.

- `frontend-delivery-flow/SKILL.md`
    - Use as default orchestration for frontend tasks.
    - Focus: strict sequence of implement -> test -> review.

## Recommended skill order

For most feature work:

1. `backend-endpoint-addition` or `integration-client-change`
2. `mongo-query-patterns` (if persistence changed)
3. `dto-contract-evolution` (if contract changed)
4. `auth-flow-touchpoints` (if auth changed)
5. `regression-test-selection`
6. `frontend-unit-testing` (if frontend changed)
7. `frontend-delivery-flow` (for frontend-first tasks)
8. `code-review`
9. `code-sense-check`

## Frontend default flow

For frontend tasks, use this order:

1. Implement the change
2. Test the change
3. Review the change

Primary orchestrator:

- `frontend-delivery-flow`

## Selection quick map

- New endpoint/use case: `backend-endpoint-addition`
- External API behavior changed: `integration-client-change`
- Mongo query/update changed: `mongo-query-patterns`
- DTO/shape changed: `dto-contract-evolution`
- JWT/OAuth changed: `auth-flow-touchpoints`
- Test scope decision needed: `regression-test-selection`
- Frontend unit coverage needed: `frontend-unit-testing`
- Frontend task with full quality gates: `frontend-delivery-flow`
- Reviewing a PR: `code-review`
- Sanity-check generated/uncertain logic: `code-sense-check`
