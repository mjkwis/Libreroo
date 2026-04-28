---
name: integration-client-change
description: Implement or review external integration client changes while preserving CXJ security and observability patterns. Use when changing mulesoft/dialog clients, headers, correlation IDs, timeout/SSL settings, and error mapping.
---

# Skill: Integration Client Change

## Purpose

Implement or review external API integration changes without breaking CXJ integration/security conventions.

## Trigger Conditions

Use this skill when editing:

- `felix-mulesoft-integration` client/facade/config classes
- `dialog-authentication` remote auth/OAuth client code
- Error mapping, headers, correlation IDs, timeout/SSL behavior for outbound calls

## Pattern Anchors (use as references)

- Shared integration config + correlation ID source:
  `felix-mulesoft-integration/src/main/java/com/roche/dia/dto/cex/felix/mulesoftintegration/base/MulesoftClient.java`
- Concrete client call pattern:
  `felix-mulesoft-integration/src/main/java/com/roche/dia/dto/cex/felix/mulesoftintegration/accountsapi/MulesoftAccountsClient.java`
- Auth remote call pattern:
  `dialog-authentication/src/main/java/com/roche/dia/dto/cex/auth/service/DialogOAuthClient.java`
- RestTemplate builder baseline:
  `felix-mulesoft-integration/src/main/java/com/roche/dia/dto/cex/felix/mulesoftintegration/base/FelixRestTemplateBuilder.java`

## Non-Negotiable Invariants

- Integration clients stay in `felix-mulesoft-integration`; auth remote flows stay in `dialog-authentication`.
- Correlation ID behavior is preserved on outbound requests.
- Credentials come from configuration, never hardcoded.
- Provider-specific payloads are mapped before crossing module/API boundaries.

## Workflow

1. Confirm changed external contract and impacted internal consumers.
2. Keep transport concerns in client classes and orchestration in core services.
3. Reuse `MulesoftClient`/shared config objects for base URIs, credentials, and headers.
4. Preserve timeout, SSL, and tracing behavior from current patterns.
5. Map errors to domain-level exceptions with useful context.
6. Add tests for happy-path and failure-path behavior.

## Review Checklist

- Correct authentication headers/token source are used.
- `x-correlation-id` propagation is maintained where expected.
- Non-2xx responses and parse failures are handled explicitly.
- No sensitive values are logged.
- Mapping code is centralized and testable.
- Backward compatibility for consuming services is evaluated.

## Anti-Patterns to Reject

- Direct external calls added in controllers.
- New one-off RestTemplate builders that bypass shared defaults.
- Swallowing integration errors without domain-level signals.
- Returning provider DTOs directly from controller responses unintentionally.

## Verification Commands

- `./gradlew :felix-mulesoft-integration:test`
- `./gradlew :dialog-authentication:test`
- `./gradlew :felix-core:test` (when orchestration logic changed)

## Done Criteria

- Integration change follows existing client architecture.
- Failure behavior is explicit and covered by tests.
- No secret/correlation/observability regressions introduced.

