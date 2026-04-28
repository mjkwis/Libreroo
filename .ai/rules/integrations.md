# Integrations and Security Rules

## 1) External integrations ownership

- External system clients belong in dedicated integration modules/adapters.
- Authentication/JWT/OAuth flows belong in dedicated authentication modules/services.
- Backend service/domain layers orchestrate integration calls as part of business workflows.

## 2) Security and credentials

- Never commit secrets, tokens, private keys, or credential material.
- Use established config/environment/secret mechanisms already present in the project.
- Keep SSL/auth client setup patterns aligned with existing integration client configuration.

## 3) Resilience and observability

- Preserve correlation/tracing behavior for external requests.
- Keep timeout and error-handling behavior explicit and consistent.
- Surface integration failures with meaningful domain/service-level exceptions.

## 4) Contract and mapping discipline

- Keep mapping from external payloads to domain models centralized and testable.
- Avoid leaking provider-specific structures into controller APIs unless explicitly intended.
- When integration contracts change, update mapping + tests + onboarding docs in the same PR.

## 5) Change checklist for integration work

- Auth path validated (token/headers/SSL assumptions).
- Error handling behavior verified (4xx/5xx/timeouts).
- Mapping correctness covered by tests.
- Backward compatibility impact reviewed for downstream consumers.

## 6) Loop guard (token control)

- If agents are ping-ponging and the same question/request appears 2 times without progress, stop the loop and ask the
  code owner (user) for a decision before continuing.
