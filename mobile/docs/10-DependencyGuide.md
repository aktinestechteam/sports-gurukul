# Dependency Guide

Status: **Adopted** · Owner: Mobile Architecture Team

## 1. Inventory

Runtime dependencies (current state, P003):

| Package | Version | Purpose | Entry point |
| --- | --- | --- | --- |
| flutter_riverpod | ^3.4.2 | state management / DI | providers (dependency container) |
| go_router | ^17.3.0 | navigation | `app/router/app_router.dart` |
| intl | 0.20.2 | l10n / date formatting | pinned (SDK constraint) |
| dio | ^5.11.0 | HTTP | `core/network/api_client.dart` |
| logger | ^2.7.0 | logging | `core/logging/app_logger.dart` |
| freezed_annotation | ^3.1.0 | immutable models | with json_annotation |
| json_annotation | ^4.12.0 | JSON codegen | with freezed_annotation |
| drift | ^2.34.3 | local SQL database | `core/database/app_database.dart` |
| drift_flutter | ^0.3.1 | drift native/web setup | `core/database/app_database.dart` |
| flutter_secure_storage | ^10.3.1 | encrypted key-value | `core/storage/secure_storage.dart` |
| shared_preferences | ^2.5.5 | plain key-value | `core/storage/preference_storage.dart` |
| connectivity_plus | ^7.3.1 | network state | `core/connectivity/connectivity_service.dart` |
| package_info_plus | ^10.2.1 | app metadata | `core/info/app_info.dart` |
| device_info_plus | ^13.2.0 | device metadata | `core/info/device_info.dart` |
| uuid | ^4.6.0 | unique ids | `core/utils/unique_id.dart` |
| collection | ^1.19.1 | collection helpers | direct use in features |

Dev dependencies:

| Package | Version | Purpose |
| --- | --- | --- |
| very_good_analysis | ^10.3.0 | lint set |
| build_runner | ^2.15.1 | codegen driver |
| freezed | 3.2.5 | model codegen (pinned) |
| json_serializable | ^6.14.1 | JSON codegen |
| drift_dev | ^2.34.0 | drift codegen |
| mocktail | ^1.0.5 | test mocks |

## 2. Adding a Dependency

1. Confirm it is needed and compatible with the freezed 3.2.5 analyzer
   constraint (analyzer >= 9 < 11).
2. `flutter pub add <package>` (runtime) or `flutter pub add --dev <package>`
   (dev-only).
3. Prefer a facade in `lib/core/` so app code does not import the package
   directly (see `AppLogger`, `ConnectivityService`, `UniqueId`).
4. Record the decision in `docs/13-PackageDecisionLog.md`.
5. Verify: `flutter analyze` → `flutter test` → `flutter build web`.

## 3. Versioning Policy

- Prefer stable releases; pre-releases only when forced, never by default.
- Pin exact versions where the Flutter SDK or another dependency constrains
  the package (e.g. `intl`, `freezed`).
- `pubspec.lock` is committed so builds are reproducible.

## 4. Known Constraints

- `freezed` 3.2.5 requires analyzer >= 9 < 11.
- `riverpod_generator` (riverpod codegen) currently conflicts with that pin
  (needs analyzer ^12/^13). Hand-written Riverpod 3 providers are used until
  the constraint clears — see decision log.
- `golden_toolkit` and `url_strategy` are discontinued; use the built-in
  `matchesGoldenFile` and `usePathUrlStrategy()` instead.
