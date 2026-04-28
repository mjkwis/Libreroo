---
name: mongo-query-patterns
description: Design or modify MongoDB repository and MongoTemplate queries using established felix-core patterns. Use when adding or changing queries, aggregations, update operations, and persistence behaviors.
---

# Skill: Mongo Query Patterns

## Purpose

Add or modify MongoDB data access in a way that matches existing `felix-core` repository/query conventions.

## Trigger Conditions

Use this skill when touching:

- Spring Data repositories in `felix-core`
- Custom repository implementations using `MongoTemplate`
- Aggregations, update operations, or query criteria for domain entities

## Pattern Anchors (use as references)

- Repository interface with default guard methods:
  `felix-core/src/main/java/com/roche/dia/dto/cex/felix/issues/InstrumentIssueRepository.java`
- Custom repository implementation with query/update patterns:
  `felix-core/src/main/java/com/roche/dia/dto/cex/felix/issues/InstrumentIssueRepositoryCustomImpl.java`
- Additional repository references from onboarding:
  `felix-core/src/main/java/com/roche/dia/dto/cex/felix/issues/history/InstrumentIssueHistoryRepository.java`

## Non-Negotiable Invariants

- Data access stays in `felix-core`.
- Domain-specific not-found/error behavior maps to existing exception types.
- Query/update logic is deterministic and explicit (no hidden side effects).
- New persistence path should not duplicate an existing repository capability.

## Workflow

1. Check whether an existing repository method already solves the use case.
2. Add method to repository interface when query is simple and declarative.
3. Use custom repository + `MongoTemplate` for advanced updates/aggregations.
4. Validate `matchedCount`/`modifiedCount` for writes and fail with domain exception when required.
5. Keep criteria readable and grouped to reflect business conditions.
6. Add focused tests for query correctness and edge cases.

## Review Checklist

- Query criteria include all required tenancy/scope fields (for example lab scoping).
- Update operations are safe for partial document updates.
- Not-found behavior is consistent with existing error codes.
- Aggregation pipelines are minimal and stable in ordering/grouping.
- No domain logic leakage into repository classes beyond query concerns.

## Anti-Patterns to Reject

- Data access logic moved into service/controller layers ad hoc.
- Broad queries without required business filters.
- Silent no-op updates that should report not-found or conflict conditions.
- Copy-pasted query logic duplicated across repositories.

## Verification Commands

- `./gradlew :felix-core:test --tests "*Repository*"`
- `./gradlew :felix-core:integrationTest --tests "*Repository*"`

## Done Criteria

- Query/write behavior is correct and covered.
- Exceptions and scope constraints match existing patterns.
- No duplicate repository abstractions were introduced.

