# Engineering Standards

Status: **Adopted** · Owner: Mobile Architecture Team

References: `docs/07-Coding-Standards.md` (architecture rules), `docs/11-NamingConvention.md` (naming), `docs/12-GitWorkflow.md` (git), `docs/10-DependencyGuide.md` (packages).

## 1. Toolchain

- Flutter 3.44 stable, Dart 3.12. `pubspec.yaml` declares `environment.sdk: ^3.8.0`.
- Formatting: `dart format`. CI enforces `dart format --set-exit-if-changed`.
- Verification gates: `flutter analyze` reports zero issues and `flutter test` is green.
- Primary dev OS is Windows; the local verification target is web
  (`flutter build web`). Android/iOS are CI release targets.

## 2. Analysis & Linting

- `analysis_options.yaml` includes `very_good_analysis` (replaces `flutter_lints`).
- Generated code is excluded from analysis:
  - `lib/l10n/generated/**`
  - `lib/**/*.g.dart`, `lib/**/*.freezed.dart`
- Documented deviations (deliberate; do not remove without discussion):
  - `public_member_api_docs: false` — internal app; class-level docs are
    required, per-member docs are optional.
  - `invalid_annotation_target: ignore` — freezed annotations on fields.
  - `formatter.trailing_commas: preserve` — `dart format` will not add
    trailing commas; add them where `require_trailing_commas` demands.
- Warnings are fixed, not suppressed. When a lint cannot be satisfied
  cleanly, add a one-line `// ignore: <lint>` with a reason and surface it in
  review.

## 3. Imports & Code Style

- Package imports only (`package:sports_gurukul/...`); no relative imports.
- No `print()` — all logging goes through `AppLogger`
  (`lib/core/logging/app_logger.dart`).
- No hardcoded user-facing strings; everything from l10n ARB + generated
  `AppLocalizations`. Run `flutter gen-l10n` after editing ARB files.
- No hardcoded colors, spacing or radius — design tokens only.

## 4. Logging Rules (AppLogger)

- Log method, URI and status only. Never log headers, payloads, tokens, PII
  or stack traces that leak secrets.
- Debug/trace messages are development-only; production verbosity is tuned in
  the facade (`lib/core/logging/app_logger.dart`).

## 5. Errors & Resilience

- Network errors map through `mapNetworkError` to `NetworkErrorKind`
  (`lib/core/network/error_mapper.dart`) at the boundary.
- Retries are centralized in `RetryInterceptor` (timeouts, connectivity,
  HTTP 429/5xx). Callers never implement their own retry loops.
- Every request carries an `X-Request-Id` correlation header
  (`RequestIdInterceptor`).
- Features surface typed failures to the UI; raw exceptions never reach
  widgets (see `docs/07-Coding-Standards.md` §7).

## 6. Storage Split

- `SecureStorage` (`lib/core/storage/secure_storage.dart`) = tokens,
  credentials, biometric references — platform keychain only.
- `PreferenceStorage` (`lib/core/storage/preference_storage.dart`) =
  non-sensitive settings (theme, language, onboarding) via shared preferences.
- The two never share a key namespace; secrets never go into prefs.

## 7. Offline & Database

- Local persistence via drift (`AppDatabase`,
  `lib/core/database/app_database.dart`).
- Offline mutations go through `OfflineQueue` (`lib/core/offline/`);
  the outbox table lands in P004.
- Schema changes add a migration step (`schemaVersion` bump); never mutate
  existing tables in place across releases.

## 8. Code Generation

- `dart run build_runner build` regenerates `.g.dart`, `.freezed.dart` and
  drift outputs. Generated files are committed and never hand-edited.
- After adding or editing an annotated model: regenerate, then verify
  analyze → test → build.

## 9. Testing

- Layout: `test/unit/` (pure logic), `test/widget/` (widgets + goldens),
  `test/integration/`, plus shared `test/fixtures/`, `test/mocks/`,
  `test/helpers/`.
- Mocks via `mocktail` (see `test/mocks/`); golden tests use the built-in
  `matchesGoldenFile` (`golden_toolkit` is discontinued — see decision log).
- New behaviour ships with tests. Coverage: `flutter test --coverage`
  produces `coverage/lcov.info` (gitignored).

## 10. Definition of Done

- [ ] `dart format --set-exit-if-changed` clean
- [ ] `flutter analyze` reports no issues
- [ ] `flutter test` green; tests added where behaviour changed
- [ ] No secrets, keys or PII committed; no `print()`; no sensitive logging
- [ ] No hardcoded user-facing strings, colors or spacing
- [ ] No cross-feature imports; shared logic promoted to `core/` or `shared/`
