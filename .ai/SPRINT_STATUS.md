# SPRINT_STATUS

Status: **Living** - Owner: Chief Software Architect
> Update this file at the start and end of every sprint/prompt.

## Current: Sprint 0 - Foundation (P004 in progress)

### Sprint goal

Establish the permanent engineering foundation and AI governance knowledge
base so feature development can proceed safely.

### Progress

| Prompt | Deliverable | Status |
| --- | --- | --- |
| P001 | Project foundation: architecture, folder structure, theming, l10n, routing, CI-ready tooling | Done |
| P002 | Bootstrap verified: splash -> placeholder dashboard; Riverpod + go_router wired; test baseline | Done |
| P003 | Engineering foundations: very_good_analysis, freezed/json codegen, dio network layer, drift scaffold, storage, utilities, testing, docs 09-13 | Done |
| P004 | AI development governance & project knowledge base (`.ai/`) | Done (this prompt) |

### Verification status (last run)

- `dart format --set-exit-if-changed` - clean
- `flutter analyze` - 0 issues
- `flutter test` - green (37 tests)
- `flutter build web` - success
- `pubspec.lock` - committed (un-ignored in P003)

### Open items before feature work

- Remove `sample_model.dart` + its generated files + unit test when real
  models land (P005).
- Make `AuthInterceptor` functional (JWT injection/refresh).
- Wire environment-based base URL in `ApiClient.create()`.
- Add `AppRadius` / `AppTypography` tokens (design-system sprint).
- Wire CI (format/analyze/test on PRs).

## Next: Sprint 1 (planned)

- Authentication & onboarding (JWT via `SecureStorage`, biometrics).
- Role-based dashboards + route guards.
- Real domain models (freezed) replacing the sample model.
- Offline-first outbox (`OfflineQueue` table) + conflict resolution.

## Rules for contributors

- Update this file at the **end** of every prompt.
- Keep the tables truthful: mark Done only after verification passes.
- Record compromises in `TECH_DEBT.md`; record decisions in `DECISIONS.md`.
