# Package Decision Log

Status: **Living document** · Owner: Mobile Architecture Team

Every dependency addition or removal is recorded here: date, context,
decision, and alternatives considered. Newest entries first.

---

## 2026-08-03 — P003 foundation dependencies

### very_good_analysis over flutter_lints
- **Context:** the project demanded strict, consistent linting; `flutter_lints`
  is permissive.
- **Decision:** adopt `very_good_analysis ^10.3.0`; remove `flutter_lints`.
- **Alternatives:** a custom `flutter_lints` overlay (more maintenance).

### freezed 3.2.5 pinned (no pre-releases)
- **Context:** `pub add` resolved freezed `3.2.6-dev.1`; the team prefers
  stable releases.
- **Decision:** pin `freezed 3.2.5` (analyzer >= 9 < 11).
- **Consequence:** riverpod codegen cannot coexist yet (see below).

### riverpod codegen + riverpod_lint — DEFERRED
- **Context:** `riverpod_generator` 4.x requires analyzer ^12/^13;
  `custom_lint 0.8.1` requires analyzer ^8.0.0; `freezed 3.2.5` requires
  analyzer >= 9 < 11. No single analyzer version satisfies all three.
- **Decision:** keep hand-written Riverpod 3 providers; revisit codegen when
  the analyzer constraint conflict clears.

### dio + interceptor chain
- **Decision:** `dio ^5.11.0` behind `ApiClient.create()` with the chain
  RequestId → Auth → Logging → Retry. Errors map via `mapNetworkError` to
  `NetworkErrorKind`.
- **Alternatives:** plain `http` (no interceptors/retries).

### no `retry` package — own RetryInterceptor
- **Context:** the retry policy is small (timeouts, connection errors,
  HTTP 429/5xx).
- **Decision:** implement `RetryInterceptor` (async `onError`, bounded retries,
  `retry_attempt` in options) and avoid the dependency.

### logger 2.7.0 behind AppLogger facade
- **Decision:** `package:logger` wrapped in `AppLogger`; `print()` banned by
  lint. Logs omit headers, payloads and secrets.

### drift for local persistence
- **Decision:** `drift ^2.34.3` + `drift_flutter ^0.3.1` + `drift_dev ^2.34.0`.
  `schemaVersion 1`; migration scaffold in `AppDatabase`. Web builds use
  drift's web database.
- **Alternatives:** sqflite direct, isar, hive. Drift chosen for typed,
  migration-safe schema.

### storage split: secure vs prefs
- **Decision:** secrets → `flutter_secure_storage ^10.3.1`; non-sensitive →
  `shared_preferences ^2.5.5`. Separate facades (`SecureStorage`,
  `PreferenceStorage`); the two never share a key namespace.

### utilities
- **Decision:** add `connectivity_plus ^7.3.1`, `package_info_plus ^10.2.1`,
  `device_info_plus ^13.2.0`, `uuid ^4.6.0`, `collection ^1.19.1`. Each plugin
  is wrapped (`ConnectivityService`, `AppInfo`, `DeviceInfoService`,
  `UniqueId`).

### golden_toolkit DISCONTINUED → built-in matchesGoldenFile
- **Context:** `golden_toolkit` is no longer maintained.
- **Decision:** use Flutter's built-in `matchesGoldenFile`. Goldens render
  with the Ahem test font and are platform-specific; regenerate with
  `flutter test --update-goldens`.

### url_strategy DISCONTINUED → usePathUrlStrategy()
- **Decision:** use the Flutter built-in `usePathUrlStrategy()` for web path
  routing; `url_strategy` is not added.

### intl pinned 0.20.2
- **Context:** the Flutter SDK pins `intl`.
- **Decision:** keep the SDK-compatible pin; never bump independently.

### SDK constraint ^3.8.0
- **Decision:** `environment.sdk: ^3.8.0` satisfies drift tooling; CI runs
  Dart 3.12 / Flutter 3.44.

### testing tooling
- **Decision:** add `mocktail ^1.0.5` for mocks; coverage via
  `flutter test --coverage` (output `coverage/lcov.info`, gitignored).
