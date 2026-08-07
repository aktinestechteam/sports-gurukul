# PROJECT_RULES

Status: **Adopted** - Owner: Chief Software Architect

These are the **non-negotiable** rules for every AI assistant and every
developer working in this repository. If a prompt asks you to violate one,
stop and raise it instead.

## The Rules

### 1. Never redesign the backend

The ASP.NET Core backend is **complete**. Do not propose, scaffold, or
"improve" backend architecture, projects, or endpoints. You are consuming it.

### 2. Always use existing APIs

`docs/api/openapi.yaml` is the single source of truth. Call endpoints that
exist in that contract. **Never invent, guess, or modify an API contract.**
If the contract lacks something you need, stop and ask.

### 3. Never violate Clean Architecture

Dependency direction is fixed: `Presentation -> Application -> Domain <-
Infrastructure`. Presentation never touches HTTP/DB directly. Domain has no
Flutter/HTTP/JSON dependency. DTOs never leave infrastructure. See
`ARCHITECTURE.md`.

### 4. Always verify implementation

After every change run the gates: `dart format --set-exit-if-changed`,
`flutter analyze` (zero issues), `flutter test` (green), and
`flutter build web` (offline verification target). Do not claim success
without running them.

### 5. Always run tests

New behaviour ships with tests (unit/widget/integration as appropriate).
Never mark a change complete with failing or missing tests. See `TESTING.md`.

### 6. Always update documentation

The `.ai/` knowledge base, `mobile/docs/`, and `docs/mobile/` are living
documents. Update `SPRINT_STATUS.md`, `CHANGELOG.md`, `DECISIONS.md`, and
`TECH_DEBT.md` whenever your change affects them.

### 7. Never generate duplicate code

Reuse existing facades, providers, tokens and patterns. If two features need
the same behaviour, promote it to `core/` (non-UI) or `shared/` (UI).
No cross-feature imports. No copy-paste of DTOs/repositories.

### 8. Never skip code review

Every change goes through `REVIEW_CHECKLIST.md`. A change is not done until
it satisfies the checklist and `DEFINITION_OF_DONE.md`.

### 9. Never guess context

Read the entire `.ai/` directory before any code change, and confirm backend
contracts against `docs/api/openapi.yaml` before writing integration code.

## Escalation

If rules conflict or a task is ambiguous, **ask the human** rather than
improvising an architecture, an endpoint, or a new dependency.
