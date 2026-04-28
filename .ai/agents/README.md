# Project Sub-Agents

Use these role profiles to route work to the right specialist.

- Backend Developer: `.ai/agents/backend-developer.md`
- Frontend Developer: `.ai/agents/frontend-developer.md`
- Architect: `.ai/agents/architect.md`
- Security: `.ai/agents/security.md`
- QA Tester: `.ai/agents/qa-tester.md`
- Release Manager: `.ai/agents/release-manager.md`
- CI/CD: `.ai/agents/cicd.md`

Routing rule:

- If a task spans multiple roles, start with `architect` for boundaries and design decisions, then hand off
  implementation to backend/frontend roles.
- If a task is clearly module-local, use the implementation role directly.

Global loop guard:

- If agents are ping-ponging and the same question/request appears 2 times without progress, stop and ask the code
  owner (user) for a decision before continuing.

## Agent routing matrix

| Issue type                                            | Primary agent        | Secondary agent(s)                        |
|-------------------------------------------------------|----------------------|-------------------------------------------|
| Backend bug (API/service/repository)                  | `backend-developer`  | `qa-tester`, `architect`                  |
| Frontend bug (UI/client flow)                         | `frontend-developer` | `qa-tester`, `backend-developer`          |
| Cross-module design/ownership question                | `architect`          | `backend-developer`, `frontend-developer` |
| Authentication/authorization problem                  | `security`           | `backend-developer`, `architect`          |
| External integration issue (third-party/internal API) | `backend-developer`  | `security`, `architect`                   |
| Test strategy / flaky tests / regression gaps         | `qa-tester`          | `backend-developer`, `frontend-developer` |
| Pipeline failure / build instability / CI speed       | `cicd`               | `qa-tester`, `release-manager`            |
| Release readiness / go-no-go decision                 | `release-manager`    | `qa-tester`, `security`, `cicd`           |
| Security finding / secret exposure risk               | `security`           | `cicd`, `release-manager`, `architect`    |
