# Backend Rules

## 1) Layering and module boundaries

- Keep API adapters/controllers thin. They should delegate to services, not implement business logic.
- Put business/use-case orchestration in backend service/domain layers.
- Keep persistence repositories and query implementations in the data-access layer.
- Keep shared domain contracts in the model/common-contract layer.
- Keep reusable technical helpers in shared utility modules.
- Keep import/batch-specific runtime logic in dedicated ingestion modules.

## 2) Data access

- Database reads/writes should be owned by the backend data-access layer.
- Prefer existing repositories and custom query patterns before adding new data access abstractions.
- Avoid introducing parallel data access paths in other modules.

## 3) API design and compatibility

- Keep endpoint behavior stable unless intentional versioned change is approved.
- Validate inputs at the boundary and keep consistent error response behavior.
- Reuse existing DTO contracts and naming conventions where possible.

## 4) Configuration and runtime

- Follow existing runtime profiles and configuration patterns.
- Do not hardcode secrets/credentials.
- Keep scheduled/background behavior aligned with existing scheduler/locking setup.

## 5) When adding new backend functionality

1. Add/extend model contracts if needed.
2. Add orchestration/service logic in backend service/domain layers.
3. Add/update API endpoint adapters if externally exposed.
4. Add tests in relevant backend test suites.

## 6) Loop guard (token control)

- If agents are ping-ponging and the same question/request appears 2 times without progress, stop the loop and ask the
  code owner (user) for a decision before continuing.
