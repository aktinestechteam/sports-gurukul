# CHANGELOG

Status: **Living** - Owner: Chief Software Architect

Chronological record of prompts/sprints. Newest first. Keep entries truthful
and verifiable.

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
