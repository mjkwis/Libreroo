---
name: code-sense-check
description: Run an independent critical sanity-check pass on produced code to verify it truly matches intended behavior. Use after implementation to challenge assumptions, validate edge cases, and confirm solution correctness.
---

# Skill: Code Sense Check

## Purpose

Cross-check whether produced code actually makes sense in business, architectural, and runtime context before merge.

This skill is an independent verification pass:

- Treat it as a separate review thread from the implementation/main thread.
- Do not trust assumptions from the main thread without re-validating them.
- Critically challenge the produced solution before acceptance.

## Trigger Conditions

Use this skill when:

- AI-generated or rapidly produced code needs sanity validation
- A change compiles but confidence in behavior is low
- Refactors introduce uncertainty about intent vs implementation
- A main-thread implementation is ready and requires independent challenge

For frontend tasks, run this as the last gate within:

- `frontend-delivery-flow` (implement -> test -> review)

## Core Questions

1. Does the code solve the stated problem, not a nearby problem?
2. Does control flow match real use cases and edge cases?
3. Do names, contracts, and side effects align with existing domain language?
4. Does it fit current CXJ boundaries and patterns?
5. Is there enough test evidence for the behavior it claims?

## Pattern Anchors (use as references)

- Controller/service layering: `felix-api/src/main/java/com/roche/dia/dto/cex/felix/cases/CaseController.java`
- Orchestration complexity baseline:
  `felix-core/src/main/java/com/roche/dia/dto/cex/felix/issues/InstrumentIssueService.java`
- Repository/query behavior baseline:
  `felix-core/src/main/java/com/roche/dia/dto/cex/felix/issues/InstrumentIssueRepositoryCustomImpl.java`
- Frontend DTO consumption baseline: `felix-universal/src/app/chatbot/services/chat-streaming.service.spec.ts`

## Workflow

1. Start a separate review thread/context and restate expected behavior in 3-5 concrete scenarios.
2. Reconstruct intent from ticket/PR and code, independent of main-thread conclusions.
3. Walk changed code path end-to-end (input -> processing -> output -> side effects).
4. Validate negative/edge paths (null, empty, invalid state, failed dependency).
5. Check contract consistency (types, DTOs, error semantics, backward compatibility).
6. Verify boundaries (API/core/repo/integration/auth ownership).
7. Confirm tests cover claims; add missing evidence as findings.

## Independence Rules (mandatory)

- Assume the main-thread solution may be wrong until verified.
- Re-check critical branches, guards, and side effects from first principles.
- Prefer contradiction over confirmation when evidence is weak.
- Raise findings even if they conflict with main-thread rationale.
- Do not mark as safe based only on successful build/tests if scenario coverage is incomplete.

## Sense-Check Checklist

- Logic correctness:
    - Conditions and branches reflect business rules, not assumptions.
    - No unreachable/contradictory branches.
- Data correctness:
    - IDs, tenant/lab scoping, and persistence keys are used consistently.
    - Updates/deletes cannot silently affect wrong records.
- Side effects:
    - Notifications, file operations, and external calls are intentional and ordered safely.
    - Failure in side effects does not leave invalid state unnoticed.
- API/UX consistency:
    - Response and error behavior match existing contract expectations.
    - Frontend-facing shape changes are coordinated.
- Maintainability:
    - Code is understandable without hidden coupling or duplicated logic.

## Output Format (mandatory)

Report:

- `High`: logic is wrong or high-risk mismatch with intended behavior
- `Medium`: plausible but brittle/incomplete behavior
- `Low`: readability or small consistency issues

For each item include:

- File and line
- Suspected mismatch
- Expected behavior
- Recommended correction or validation test

If no issues are found:

- Explicitly state the checked scenarios and residual uncertainty.
- Explicitly state that this was an independent review pass.

## Anti-Patterns to Reject

- "Compiles, therefore correct" acceptance.
- Trusting generated code without scenario-based validation.
- Merging behavior changes without explicit end-to-end walkthrough.

