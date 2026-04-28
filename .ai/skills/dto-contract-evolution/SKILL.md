---
name: dto-contract-evolution
description: Evolve backend/frontend DTO contracts safely through CXJ generation flow. Use when API shapes change, backend contract classes are updated, or frontend dto.ts consumers must be synchronized.
---

# Skill: DTO Contract Evolution

## Purpose

Evolve backend-frontend contracts safely using the existing generated DTO pipeline and compatibility rules.

## Trigger Conditions

Use this skill when a change:

- Adds or modifies contract classes consumed by frontend
- Changes API request/response shape
- Requires new enums/types in generated TypeScript DTOs

## Pattern Anchors (use as references)

- DTO generation configuration: `felix-core/build.gradle`
- Generated frontend DTO output target: `felix-universal/src/app/shared/models/dto.ts`
- Frontend consumption pattern: `felix-universal/src/app/chatbot/services/chat-streaming.service.spec.ts`
- Broad import usage (shared model entrypoint): `felix-universal/src/app`

## Non-Negotiable Invariants

- Backend contracts are the source of truth.
- `dto.ts` is generated from backend configuration, not manually handcrafted.
- Public API compatibility remains stable unless intentionally changed and documented.
- DTO changes are synchronized with frontend usage in the same change set when needed.

## Workflow

1. Identify whether existing DTO/model already satisfies new behavior.
2. Update backend contract class only when required.
3. Include/exclude class in `generateTypeScript` config if frontend must consume it.
4. Regenerate TypeScript DTO artifacts from backend build flow.
5. Update frontend compilation/runtime usage and tests.
6. Document contract impact and migration notes for breaking changes.

## Review Checklist

- Naming and field semantics align with existing model language.
- Optional vs required fields are intentional and compatible.
- Enum changes are backward-safe for existing frontend logic.
- No duplicate parallel DTO introduced in feature-local frontend files.
- Serialization annotations/behavior remain consistent.

## Anti-Patterns to Reject

- Manual edits in generated DTO output that are not reproducible.
- Contract changes without frontend updates where usage exists.
- Breaking endpoint shape silently without deprecation/version plan.

## Verification Commands

- `./gradlew :felix-core:generateTypeScript`
- `./gradlew :felix-core:test`
- `cd felix-universal && npm run lint && npm test`

## Done Criteria

- Generated DTOs are up to date and consumed successfully.
- Backend and frontend compile/test for touched scope.
- Contract change risk is documented clearly.

