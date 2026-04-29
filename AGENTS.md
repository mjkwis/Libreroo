# AGENTS Guide for Libreroo

This is the primary contributor guide for engineers and coding agents working in this repository.

## 1) Start here

- Read `README.md` for local run basics and ports.
- Read `docs/architecture.md` for modular-monolith boundaries and dependency rules.
- Read `docs/techstack.md` for platform/tooling decisions.
- Read `docs/adr/README.md` and existing ADRs before changing architectural direction.
- Use this file as the top-level policy.

## 2) Rule structure and precedence

Use a hybrid model:

1. `AGENTS.md` (this file): canonical high-level rules and non-negotiables.
2. Domain-specific rules in `.ai/rules/*.md`: implementation details.
3. Existing project docs (`README.md`, module docs, ADRs, architecture docs).

If rules conflict:

1. `AGENTS.md` wins.
2. Domain rule files win over generic docs.
3. Existing code patterns win over personal preferences.

Important context:

- Some `.ai` documents still contain terminology from an older project.
- For Libreroo work, prefer real repository paths and boundaries from this file and `docs/architecture.md`.

AI access control:

- Sensitive paths for AI tooling should be listed in `.aiignore`.

Temporary allowlist process:

1. Only allowlist the minimum required path for the specific task.
2. State explicitly in the task request which ignored path can be accessed and why.
3. Limit access to the duration of that task, then restore the ignore rule.
4. Do not allowlist secrets wholesale; prefer sanitized files whenever possible.

## 3) Ownership map (important)

- Backend runtime and API host: `apps/libreroo-api`
  - ASP.NET Core entrypoint (`Program.cs`), middleware wiring, migrations, and endpoint hosting.
- Backend module implementation: `apps/libreroo-api/Modules/*`
  - Modules: `Catalog`, `Members`, `Loans`.
  - Internal layers per module: `Api`, `Application`, `Domain`, `Infrastructure`.
- Shared backend cross-cutting code: `apps/libreroo-api/Shared/*`
  - Exception handling, shared errors, persistence context and common wiring.
- Frontend app: `apps/libreroo-web`
  - Angular app shell, routes, feature pages, API services, and UI behavior.
- Test ownership:
  - API/integration coverage: `tests/Libreroo.Api.Tests`
  - Domain behavior coverage: `tests/Libreroo.Domain.Tests`
- Architecture decision records:
  - `docs/adr/*`

## 4) Non-negotiables

- Keep module boundaries explicit inside `apps/libreroo-api/Modules`.
- Do not put business rules in controllers; controllers map HTTP concerns and delegate to application services.
- `Api` layer must not access EF/persistence details directly; persistence belongs in `Infrastructure` and shared context.
- Keep domain invariants in `Domain`/`Application` (for example loan borrow/return rules), not in the frontend.
- Reuse existing contracts/models before creating new duplicate shapes.
- Maintain backward compatibility for public API behavior unless a breaking change is intentional and documented.
- Do not commit secrets, tokens, or credentials; follow existing config/environment patterns.
- Respect existing formatting/tooling:
  - Frontend follows the local Prettier config (`printWidth: 100`).
  - Backend/frontend should remain formatter/linter clean for touched scope.
- ADR `0002-no-authentication-in-phase-1` applies: do not introduce auth/security model changes without explicit ADR/user approval.
- Loop guard (token control): if agents are ping-ponging and the same question/request appears 2 times without progress,
  stop the loop and ask the code owner (user) for a decision before continuing.

## 5) Change workflow

1. Identify affected app/module/layer using the ownership map above.
2. Implement changes in the proper layer (`Api` vs `Application` vs `Domain` vs `Infrastructure`, or frontend feature/core).
3. Add/update tests in the corresponding suite.
4. Run minimal local verification for changed scope.
5. Update docs/ADRs when boundaries, contracts, or architectural assumptions change.

## 6) Pull request checklist

- Scope is limited to the right module(s) and layer(s).
- New behavior is covered by tests (unit/integration where appropriate).
- No duplicate model/service/utility introduced where existing ones can be reused.
- API/contract changes are documented and reflected in frontend usage when needed.
- Security and data-handling implications reviewed.
- Frontend/backend compatibility checked when endpoint or DTO shape changes.
- Include a short testing statement (what was run, what was not, and residual risk).

## 7) Domain rule files

- Backend rules: `.ai/rules/backend.md`
- Frontend rules: `.ai/rules/frontend.md`
- Testing rules: `.ai/rules/testing.md`
- Integrations and security rules: `.ai/rules/integrations.md`

## 8) Sub-agent profiles

- Catalog: `.ai/agents/README.md`
- Backend Developer: `.ai/agents/backend-developer.md`
- Frontend Developer: `.ai/agents/frontend-developer.md`
- Architect: `.ai/agents/architect.md`
- Security: `.ai/agents/security.md`
- QA Tester: `.ai/agents/qa-tester.md`
- Release Manager: `.ai/agents/release-manager.md`
- CI/CD: `.ai/agents/cicd.md`

## 9) Skills

- Catalog: `.ai/skills/README.md`
- Code Review: `.ai/skills/code-review/SKILL.md`
- Backend Endpoint Addition: `.ai/skills/backend-endpoint-addition/SKILL.md`
- Integration Client Change: `.ai/skills/integration-client-change/SKILL.md`
- Mongo Query Patterns: `.ai/skills/mongo-query-patterns/SKILL.md`
- DTO Contract Evolution: `.ai/skills/dto-contract-evolution/SKILL.md`
- Auth Flow Touchpoints: `.ai/skills/auth-flow-touchpoints/SKILL.md`
- Regression Test Selection: `.ai/skills/regression-test-selection/SKILL.md`
- Frontend Unit Testing: `.ai/skills/frontend-unit-testing/SKILL.md`
- Frontend Delivery Flow: `.ai/skills/frontend-delivery-flow/SKILL.md`
- Code Sense Check: `.ai/skills/code-sense-check/SKILL.md`

### How to trigger skills

- Explicit trigger in request:
  - Mention the exact skill name (for example: `use backend-endpoint-addition`).
  - You can also use prefixed form (for example: `$backend-endpoint-addition`).
- Implicit trigger by task intent:
  - If a request clearly matches a skill scope, that skill should be applied even without explicit naming.
- Multiple skills:
  - Use only the minimal set needed for the task.
  - Apply in practical order (implementation skill -> test selection -> code review).
  - For frontend tasks, default to `frontend-delivery-flow` (implement -> test -> review).
- If a skill is missing or cannot be loaded:
  - State that briefly and continue with the closest applicable project rules.
