# CHANGELOG

Status: **Living** - Owner: Chief Software Architect

Chronological record of prompts/sprints. Newest first. Keep entries truthful
and verifiable.

## SGM-0002 - Enterprise Flutter Development Environment (2026-08-03)

- Built the reusable `lib/core/` foundation for the enterprise dev
  environment (no business features): `config/` (Environment/Flavor/
  BuildEnvironment, ApplicationMetadata, AppConfig), `constants/`,
  `failures/` (sealed `BaseFailure` hierarchy), `exceptions/` (sealed
  `AppException`), `result/` (sealed `Result<T>`/`OperationResult`),
  `logging/` (AppLogger facade over `package:logger` + Debug/Release),
  `extensions/`, `validators/` (composable validator framework),
  `utils/` (date, formatter, parser, file/image, platform, permission,
  debouncer).
- Kept existing app-layer imports compiling via delegates: `AppConfig`,
  `Environment`/`Flavor` and `RoutePaths` now re-export/typedef the core
  equivalents.
- Rewired `ApiClient` to be result-aware (`mapNetworkError` ->
  `NetworkErrorKind` -> `NetworkFailure`) with the full interceptor chain.
- Added unit + widget coverage (134 new tests -> 171 total) and kept the
  existing splash/dashboard/theme golden suite green.
- Verification: format clean, `flutter analyze` 0 issues, `flutter test`
  green (171/171).

## SGM-0001 - Flutter Project Bootstrap Verification (2026-08-03)

- Re-verified the existing `mobile/` bootstrap (P001-P004) against the approved
  `.ai/` governance and this task's deliverables.
- Confirmed in scope: Flutter project (`mobile/`), folder structure, README,
  `analysis_options.yaml` (very_good_analysis), `pubspec.yaml` (package name
  `sports_gurukul`, retained per CODING_STANDARDS), placeholder splash and
  placeholder dashboard.
- Confirmed out of scope and not implemented: authentication, API integration,
  Riverpod business logic, database, notifications, AI.
- Package name `sports_gurukul_mobile` (task wording) was reconciled with
  governance: kept `sports_gurukul` after human confirmation (62 imports +
  adopted standard).
- Verified: `flutter pub get`, format clean, analyze 0 issues, 37/37 tests,
  `flutter build web` OK, `flutter run` (web-server) launches successfully.

## P004 - AI Development Governance & Project Knowledge Base (2026-08-03)

- Created `.ai/` governance knowledge base (24 documents).
- Added project context, rules, architecture, folder structure, coding /
  Flutter standards, state management, networking, database, design system,
  UI guidelines, backend integration, API guidelines, security, performance,
  testing, git workflow, sprint status, tech debt, ADR decisions, changelog,
  prompt template, review checklist, definition of done.
- Updated root and mobile READMEs with governance pointers.
- Verified: format/analyze/test/build-web all green.

## P003 - Engineering Standards & Dependencies (2026-08-03)

- Adopted very_good_analysis (removed flutter_lints); fixed all lint issues;
  package imports only; l10n generated excluded from analysis.
- Added freezed 3.2.5 + json_serializable + build_runner; validated codegen
  with `sample_model.dart`.
- Deferred riverpod codegen (analyzer conflict; recorded as ADR-004).
- Added dio 5.11 + logger 2.7: `ApiClient.create()` with RequestId -> Auth ->
  Logging -> Retry interceptors; `mapNetworkError` -> `NetworkErrorKind`.
- Added drift 2.34 + drift_flutter + drift_dev; `AppDatabase` scaffold;
  `OfflineQueue` scaffold.
- Added storage: `SecureStorage` (keychain) + `PreferenceStorage` (prefs).
- Added utilities: `ConnectivityService`, `AppInfo`, `DeviceInfoService`,
  `UniqueId`, `collection`.
- Added testing: mocktail, `SecureStorage` mock tests, golden test via
  `matchesGoldenFile`, coverage validated.
- Authored docs `mobile/docs/09`-`13`; un-ignored `pubspec.lock`.
- Verification: format clean, analyze 0 issues, 37/37 tests, build web OK.

## P002 - Enterprise Project Architecture (bootstrap verified)

- App boots splash -> placeholder dashboard.
- Riverpod 3 + go_router wired; centralized routes (RouteNames/RoutePaths)
  and route guards scaffold.
- Material 3 theme from seed `Color(0xFF006DFF)`; l10n (en, hi, mr).
- Test baseline established (app bootstrap + theme tests).
- `mobile/docs/01`-`08` authored.

## P001 - Project Bootstrap (foundation)

- Flutter project scaffolded under `mobile/`.
- Clean Architecture + Feature First folder structure.
- Theming, localization, routing, CI-ready tooling.
- Core sprint docs and `docs/mobile/` product specs in place.

## Sprint 0 - scope

- P001 foundation -> P002 bootstrap verification -> P003 engineering standards
  -> P004 AI governance knowledge base.
- Feature work (auth, role-based modules) begins with Sprint 1 (P005+).
