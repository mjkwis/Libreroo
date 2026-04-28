# Libreroo Phase 1 Frontend Public App Design

## Context

This spec defines the Phase 1 frontend built on top of the existing backend foundation:

- `docs/superpowers/specs/2026-04-28-libreroo-backend-foundation-design.md`
- `.ai/libreroo_concept.md`
- `.ai/rules/frontend.md`

Phase 1 frontend objective is a simple but credible public member app that exercises backend flows without introducing
authentication yet.

## Scope

### In scope

- Create a new Angular frontend application in `apps/libreroo-web`.
- Build public member-facing flows without login/auth:
  - browse books
  - select member identity from existing members
  - quick member registration inline
  - borrow a book
  - list active loans
  - return a loan
- Integrate with existing backend endpoints:
  - `GET /books`
  - `GET /members`
  - `POST /members`
  - `POST /loans`
  - `GET /loans/active`
  - `POST /loans/{id}/return`

### Out of scope (phase 1)

- Authentication/authorization UI and token handling
- Admin CRUD UI for books/authors/members
- Advanced frontend features (pagination, search, filtering, i18n)
- UI redesign/polish beyond functional product-grade baseline

## Approach Selection

Chosen approach: **A - Separate Angular SPA in `apps/libreroo-web`**.

Alternatives considered:

1. Angular SPA served independently (chosen)
2. Angular build embedded/served by API project
3. Server-rendered MVC/Razor UI in API project

Rationale for choice:

- Preserves clean frontend/backend separation.
- Best alignment with project intent (Angular + .NET).
- Easiest path to evolve into auth-enabled Phase 2 app.

## Architecture

Frontend is a route-based Angular SPA with thin API clients and minimal client state.

### Route surface

- `/catalog` - book browsing and borrow action
- `/member` - member selection and quick registration
- `/loans` - active loans and return action

### Layering

- **Pages/components**: presentation and interaction handlers.
- **API services**: typed wrappers around backend endpoints.
- **Local state service**: selected member context (`id`, `fullName`) persisted in `localStorage`.
- **Error mapping utility/service**: normalize API errors to stable UI messaging.

### Boundary rule

Frontend must not reimplement backend domain rules. UI may disable impossible actions for UX, but backend remains source
of truth for business invariants.

## Components and Data Flow

### 1) Catalog Page

- Loads books from `GET /books`.
- Renders: `title`, `authorId` (phase 1 backend shape), `availableCopies`.
- Borrow action:
  - Requires selected member from local state.
  - Calls `POST /loans` with:
    - `bookId`
    - `memberId`
    - `borrowDateUtc` (client UTC timestamp)
  - On success: refresh books list and show success notification.
  - On failure: show mapped backend error.

### 2) Member Page

- Loads members from `GET /members`.
- Allows selecting active member.
- Persists selected member in `localStorage` via shared state service.
- Quick registration:
  - Inline form with `fullName`.
  - Calls `POST /members`.
  - On success: refresh list and auto-select newly created member.

### 3) Loans Page

- Loads active loans from `GET /loans/active`.
- Displays: `id`, `bookId`, `memberId`, `borrowDate`.
- Return action:
  - Calls `POST /loans/{id}/return`.
  - On success: refresh active loans and show success notification.
  - On failure: show mapped backend error.

## Error Handling and UX Constraints

### Error handling

- Components use consistent API error mapping.
- Domain/business failures returned from backend are surfaced directly when safe:
  - `No available copies.`
  - `Member not found.`
  - `Loan already returned.`
- Network and unexpected failures return generic fallback text:
  - `Request failed. Please retry.`

### UX constraints

- Borrow action disabled when:
  - no active member is selected
  - `availableCopies <= 0`
- Return action disabled while request is in flight.
- Quick registration validates non-empty `fullName` before submit.
- No optimistic updates for borrow/return in phase 1.
  - Always refresh from server after successful mutations.

## Testing Strategy (Phase 1 Frontend)

- Component tests for:
  - borrow action gating and request payload
  - member selection persistence behavior
  - quick registration happy path + validation
  - return action flow in loans page
- API service tests for request/response contract mapping and error surface.
- Minimal integration-level frontend check:
  - app boot
  - route navigation
  - endpoint calls wired to configured API base URL

Testing remains focused on highest-risk user flows (member selection, borrow, return).

## Acceptance Criteria

- Angular app in `apps/libreroo-web` runs locally.
- User can browse books via backend data.
- User can select existing member as active context.
- User can create a member inline and immediately use it.
- Borrow operation succeeds when copies are available and selected member exists.
- Active loans are visible in UI.
- Return operation succeeds from UI and updates active loans list.
- Frontend surfaces backend business-rule errors clearly.

## Risks and Mitigations

- Risk: no-auth frontend allows identity spoofing in phase 1.
  - Mitigation: explicit temporary behavior; phase 2 introduces auth.
- Risk: backend DTO evolution may break frontend assumptions.
  - Mitigation: isolate calls in API services and keep contract-mapping tests.
- Risk: users confuse public/member UX with admin operations.
  - Mitigation: keep scope narrow and route labels explicit (catalog/member/loans only).

## Phase 2 Forward Path

- Replace member selection with authenticated identity.
- Convert selected-member local state to authenticated session context.
- Extend catalog/loan views with richer relational display once backend returns richer DTOs.
