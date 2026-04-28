# Testing Rules

## 1) Test ownership map

- Backend unit/integration tests live with backend modules.
- UI/regression suites live in dedicated frontend/e2e/regression test projects.
- Keep shared test helpers and fixtures in common test-support libraries.

## 2) Coverage expectations

- New behavior should include tests at the lowest useful level.
- Bug fixes should include a regression test where practical.
- Do not rely only on manual verification for non-trivial logic changes.

## 3) Practical strategy

- For backend logic changes: add/update module tests first, then integration tests as needed.
- For endpoint contract changes: validate controller/service behavior and update affected UI/e2e tests.
- For UI flow changes: update frontend tests and relevant e2e suites.

## 4) Execution guidance

- Prefer targeted test runs during development.
- Run broader relevant suite before merging changes touching shared flows.
- Keep flaky-test risk low by avoiding brittle selectors/timing assumptions in UI automation.

## 5) Minimum PR testing statement

Each PR should clearly state:

- Which tests were run.
- Which areas were intentionally not run (if any).
- Known risks/gaps that remain.

## 6) Loop guard (token control)

- If agents are ping-ponging and the same question/request appears 2 times without progress, stop the loop and ask the
  code owner (user) for a decision before continuing.
