---
name: frontend-delivery-flow
description: Enforce the CXJ frontend delivery sequence: implement first, test second, review last. Use when executing frontend tasks in felix-universal to ensure consistent quality gates and no skipped phases.
---

# Skill: Frontend Delivery Flow

## Purpose

Orchestrate frontend work in a strict sequence:

1. Implement
2. Test
3. Review

Do not reorder or skip phases unless explicitly requested by the code owner.

## Trigger Conditions

Use this skill when:

- A task primarily targets `felix-universal`
- Frontend feature, bugfix, refactor, or UI behavior is requested
- You want a default, repeatable quality flow for frontend delivery

## Mandatory Sequence

### Phase 1: Implement

- Implement the requested frontend behavior first.
- Keep scope focused and aligned with existing Angular/Ionic patterns.
- Reuse existing components/services/utilities before introducing abstractions.

Recommended companion skills:

- `frontend-unit-testing` (prepare scenarios while implementing)
- `dto-contract-evolution` (if contract shape changes)

Exit gate:

- Code for requested behavior is complete and compiles.

### Phase 2: Test

- Add/update unit tests for changed behavior.
- Run frontend lint and unit tests.
- Expand to broader validation only when risk requires it.

Recommended companion skills:

- `frontend-unit-testing`
- `regression-test-selection`

Minimum commands:

- `cd felix-universal && npm run lint`
- `cd felix-universal && npm test`

Exit gate:

- Relevant tests pass, or failures/gaps are explicitly documented.

### Phase 3: Review

- Perform critical review only after implementation and testing.
- Review for architecture fit, maintainability, correctness, and regressions.
- Run independent sanity-check on produced logic.

Recommended companion skills:

- `code-review`
- `code-sense-check`

Exit gate:

- Findings are documented and addressed, or deferred with explicit risk notes.

## Output Contract

Report results in this order:

1. Implementation summary
2. Testing evidence (what ran, what did not, why)
3. Review findings (severity ordered) and residual risk

## Anti-Patterns to Reject

- Reviewing before code exists.
- Skipping tests and claiming readiness.
- Marking task complete without independent review pass.
