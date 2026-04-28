---
name: frontend-unit-testing
description: Design and implement focused frontend unit tests in felix-universal with Angular testing patterns, stable mocks, and contract-aligned assertions. Use when adding or changing frontend logic, components, services, guards, pipes, or HTTP flows.
---

# Skill: Frontend Unit Testing

## Purpose

Create reliable, maintainable frontend unit tests that validate behavior in `felix-universal` without brittle setup.

## Trigger Conditions

Use this skill when:

- Changing frontend services/components/guards/pipes
- Updating client-side logic, validation, or state behavior
- Modifying HTTP request/response handling in frontend services
- Fixing frontend bugs that need regression tests

For full execution order on frontend work, run with:

- `frontend-delivery-flow` (implement -> test -> review)

## Pattern Anchors (use as references)

- Service HTTP test pattern: `felix-universal/src/app/chatbot/services/chat-streaming.service.spec.ts`
- Component test baseline: `felix-universal/src/app/app.component.spec.ts`
- Guard test baseline: `felix-universal/src/app/authentication/guards/feature.guard.spec.ts`
- Shared DTO imports in tests: `felix-universal/src/app/shared/models/dto.ts`

## Non-Negotiable Invariants

- Assert behavior, not implementation details.
- Keep tests deterministic (no timing/network flakiness).
- Reuse existing mocks/utilities before creating new helpers.
- Keep backend contract assumptions aligned with generated DTO models.

## Workflow

1. Identify the behavior change and define test scenarios (happy path + edge cases).
2. Select narrow test scope (service/component/guard/pipe) with minimal setup.
3. Mock dependencies explicitly (providers, spies, HTTP mocks).
4. Assert observable outputs, state transitions, rendered behavior, and error handling.
5. Add regression test for bug fixes.
6. Run unit tests and lint for touched frontend scope.

## Checklist

- Setup is small and readable.
- Inputs/outputs are explicit and scenario-driven.
- Error/empty/null paths are covered where relevant.
- HTTP tests verify method, URL, payload, and response mapping.
- No brittle assertions on private internals.
- Naming follows existing `should ...` style.

## Anti-Patterns to Reject

- Snapshot-style assertions with low signal.
- Over-mocking everything so behavior is not truly exercised.
- Testing framework internals instead of app logic.
- Leaving changed logic without unit coverage.

## Verification Commands

- `cd felix-universal && npm run lint`
- `cd felix-universal && npm test`

## Done Criteria

- New/changed frontend behavior is covered by focused unit tests.
- Tests are stable and readable.
- Regressions are detectable through automated test runs.
