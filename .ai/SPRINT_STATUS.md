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
| P004 | AI development governance & project knowledge base (`.ai/`) | Done |
| SGM-0001 | Bootstrap verification (this task): Flutter project, folder structure, README, `analysis_options.yaml`, `pubspec.yaml`, placeholder splash + dashboard; all gates green; package name stays `sports_gurukul` per governance | Done |
| SGM-0002 | Enterprise Flutter development environment: reusable `lib/core/` foundation (config/env, constants, failures/exceptions, sealed `Result`/`OperationResult`, logging facade, extensions, validators, utils, result-aware `ApiClient`); app-layer delegates (`AppConfig`, `Environment`, `RoutePaths`) keep existing callers compiling; 171 tests green | Done |
| SGM-0003 | Authentication feature (`lib/features/authentication/`, Clean Architecture layers) consuming the real backend auth endpoints only: login/register/refresh/logout/forgot/reset/send-verification-email/verify-email; functional `AuthInterceptor` (JWT attach + single-flight refresh + retry); splash session restore; guarded router (login/forgot/reset + dashboard w/ logout); l10n (en/hi/mr); 207 tests green; `docs/api/openapi.yaml` divergence noted | Done |
| SGM-0004 | Mobile login CORS fix: backend now allows any localhost origin in Development (Flutter web random port) while Production keeps the strict allowlist; verified preflight behavior in Dev + Prod; login 500 traced to Postgres not running (env issue, `docker compose up -d postgres`) | Done |

### Verification status (last run)

- `dart format --set-exit-if-changed` - clean
- `flutter analyze` - 0 issues
- `flutter test` - green (207 tests)
- `flutter gen-l10n` + `dart run build_runner build` - success
- `flutter build web` - success
- `flutter run` (web-server) - serves `lib/main.dart`; app boots splash -> dashboard
- `pubspec.lock` - committed (un-ignored in P003)

### Open items before feature work

- Remove `sample_model.dart` + its generated files + unit test when real
  models land (P005).
- Register/forgot-password/reset-password/send/verify-email UI pages (backend
  endpoints verified; only login/forgot/reset UI built so far).
- Verify OTP, change password, current-user and refresh-session endpoints are
  NOT exposed by the backend; UI for them is deliberately absent.
- Add `AppRadius` / `AppTypography` tokens (design-system sprint).
- Wire CI (format/analyze/test on PRs).

## Next: Sprint 1 (planned)

- Authentication UI completion (register, email verification) + biometrics
  unlock (JWT via `SecureStorage`).
- Role-based dashboards + route guards.
- Real domain models (freezed) replacing the sample model.
- Offline-first outbox (`OfflineQueue` table) + conflict resolution.

## Rules for contributors

- Update this file at the **end** of every prompt.
- Keep the tables truthful: mark Done only after verification passes.
- Record compromises in `TECH_DEBT.md`; record decisions in `DECISIONS.md`.
