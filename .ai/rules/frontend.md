# Frontend Rules

## 1) Scope and responsibility

- The frontend application owns UI behavior, user interaction flows, client-side validation, and presentation.
- Backend/domain rules remain server-side; avoid duplicating backend business logic in frontend where possible.

## 2) API and DTO contract alignment

- Treat backend contracts as source of truth.
- Respect contract generation/sync flow from backend into frontend models when present.
- Coordinate any endpoint/DTO changes with backend updates in the same change set when possible.

## 3) Code quality

- Keep lint passing for touched scope.
- Prefer small, focused component/service changes.
- Reuse existing shared frontend utilities/components before adding new abstractions.

## 4) Build and runtime expectations

- Use existing npm scripts and Gradle integration tasks.
- Keep compatibility with the current frontend stack used in this repository.
- Avoid introducing toolchain/version drift without explicit upgrade work.

## 5) Frontend change checklist

- UI behavior verified locally for changed flows.
- API calls and error handling aligned with backend behavior.
- Affected tests/lint updated and passing for changed scope.
- Mobile-specific behavior considered if change affects shared app paths.

## 6) Loop guard (token control)

- If agents are ping-ponging and the same question/request appears 2 times without progress, stop the loop and ask the
  code owner (user) for a decision before continuing.
