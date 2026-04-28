---
name: regression-test-selection
description: Select risk-based, minimal high-confidence validation scope across CXJ test suites. Use when deciding what unit, integration, regression, and e2e tests to run for a change.
---

# Skill: Regression Test Selection

## Purpose

Select the smallest test set that gives high confidence for a given change, using existing CXJ test architecture.

## Trigger Conditions

Use this skill when preparing validation scope for:

- Feature additions
- Bug fixes
- Contract changes
- Integration/auth or cross-module changes

## Pattern Anchors (use as references)

- Backend module test + integration source set pattern: `felix-core/build.gradle`
- Regression suite tasks: `felix-regression/build.gradle`
- Validation suite grouping (High/Medium/Smoke/NewFeatures): `online-support-validation/build.gradle`
- Playwright commands: `online-support-validation-playwright/package.json`
- Frontend unit test style: `felix-universal/src/app/chatbot/services/chat-streaming.service.spec.ts`
- Backend Spock style:
  `felix-core/src/test/groovy/com/roche/dia/dto/cex/felix/chats/conversations/ChatConversationsStreamingServiceSpec.groovy`

## Risk-to-Suite Mapping

- `Low risk` (localized logic/refactor, no contract change):
    - Module unit tests for touched area
    - Lint/type checks for touched frontend files

- `Medium risk` (service/repository changes, endpoint behavior changes):
    - Backend unit + integration tests for touched modules
    - Relevant frontend unit tests if API usage is affected

- `High risk` (auth, integration, shared contracts, cross-module flow):
    - Backend unit + integration tests
    - At least one regression suite path (`felix-regression` or `online-support-validation`)
    - Playwright flow tests where user journey is affected

## Workflow

1. Classify risk by changed modules and contract impact.
2. Start with fastest targeted module tests.
3. Add cross-module/integration suites as risk increases.
4. Add UI/e2e coverage when frontend flow or API contract changed.
5. Record exactly what was run and intentionally skipped.

## Command Baseline

- Backend:
    - `./gradlew :felix-core:test`
    - `./gradlew :felix-core:integrationTest`
    - `./gradlew :felix-api:test`

- Frontend:
    - `cd felix-universal && npm run lint`
    - `cd felix-universal && npm test`

- Regression:
    - `./gradlew :felix-regression:acceptanceTest`
    - `./gradlew :online-support-validation:SmokeTest`
    - `cd online-support-validation-playwright && npm test`

## Minimum PR Test Statement Template

- Tests run:
    - `<list exact commands>`
- Not run:
    - `<list skipped suites and why>`
- Residual risk:
    - `<brief risk statement>`

## Anti-Patterns to Reject

- “Build passed” without tests for behavioral changes.
- Running only UI tests for backend-heavy changes.
- Running only unit tests for high-risk auth/integration/contract changes.

## Done Criteria

- Test scope matches change risk.
- Results and gaps are explicitly documented.
- Remaining risk is visible to reviewers before merge.

