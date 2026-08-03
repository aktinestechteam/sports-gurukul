# CODING_STANDARDS

Status: **Adopted** - Owner: Chief Software Architect

The authoritative rule set for writing Dart/Flutter code in this repo.
Companions: `FLUTTER_STANDARDS.md` (Flutter-specific), `mobile/docs/07-Coding-Standards.md`,
`mobile/docs/09-EngineeringStandards.md`.

## 1. Toolchain & verification gates

- Flutter 3.44 stable / Dart 3.12; `environment.sdk: ^3.8.0` in `pubspec.yaml`.
- Every change must pass, in order:
  1. `dart format --set-exit-if-changed lib test integration_test`
  2. `flutter analyze` -> **zero issues**
  3. `flutter test` -> green
  4. `flutter build web` (offline verification target on Windows)

## 2. Formatting & linting

- Formatted with `dart format`.
- `analysis_options.yaml` includes **very_good_analysis** (strict; replaces
  `flutter_lints`). Fix warnings; do not suppress with ignores.
- Excluded from analysis (generated): `lib/l10n/generated/**`,
  `lib/**/*.g.dart`, `lib/**/*.freezed.dart`.
- Documented deviations (keep in mind; do not silently extend):
  - `public_member_api_docs: false` - class-level docs required, per-member optional.
  - `invalid_annotation_target: ignore` - freezed annotations on fields.
  - `formatter.trailing_commas: preserve` - add trailing commas where
    `require_trailing_commas` demands them; `dart format` will not add them.
- When a lint cannot be satisfied cleanly, add a one-line
  `// ignore: <lint>` with a reason and surface it in review.

## 3. Imports & style

- **Package imports only** (`package:sports_gurukul/...`); no relative imports.
- No `print()` - all logging through `AppLogger`
  (`lib/core/logging/app_logger.dart`). See `NETWORKING.md` logging rules.
- No hardcoded user-facing strings - l10n ARB + generated `AppLocalizations`;
  run `flutter gen-l10n` after editing ARB files.
- No hardcoded colors, spacing, radius or typography - design tokens only.
- No trailing whitespace; one blank line between logical blocks; no unused
  imports (`dart fix --apply` for autofixes).

## 4. Architecture rules (reminders)

- Presentation never calls APIs or databases directly.
- Domain has no Flutter/HTTP/JSON dependency.
- DTOs never leave infrastructure.
- Nothing in `core/` or `shared/` depends on a feature; no cross-feature imports.
- No global service locator; dependencies are constructor-injected via
  Riverpod providers.

## 5. Naming

See `mobile/docs/11-NamingConvention.md`. Key rules:

- Files `snake_case.dart`; classes `PascalCase`; members `camelCase`.
- Private members prefixed `_`; never expose mutable state publicly.
- Use cases `VerbNoun` (`LoginUser`); repositories `XxxRepository` /
  `XxxRepositoryImpl`; DTOs `XxxDto`; failures `XxxFailure`.
- Extensions `<ExtendedType>X` in `lib/core/extensions/`.
- Boolean setters are `set` properties (`set active(bool)`) - not `setActive()`.
- Boolean getters use `isXxx` / `hasXxx`.

## 6. Immutability & models

- Models are immutable `freezed` classes (`@freezed`), JSON via
  `json_serializable`. Regenerate with `dart run build_runner build`.
- Constants holders: `abstract final class` + `static const`.

## 7. Errors & resilience

- Network errors map through `mapNetworkError` -> `NetworkErrorKind`.
- Retries are centralized in `RetryInterceptor`; callers never retry manually.
- Features return typed `Failure` objects (feature `domain/failures/`);
  raw exceptions never reach widgets.

## 8. Security

- Never commit secrets, API keys, or tokens; inject via environment config
  and `SecureStorage`.
- HTTPS only; sensitive data never logged; PII minimized in logs.

## 9. Testing

- `test/unit/` pure logic, `test/widget/` widgets+goldens,
  `test/integration/` flows; shared `test/fixtures/`, `test/mocks/`,
  `test/helpers/`. New logic ships with tests. See `TESTING.md`.

## 10. Reference

- Engineering standards: `mobile/docs/09-EngineeringStandards.md`
- Coding standards (sprint docs): `mobile/docs/07-Coding-Standards.md`
- Naming: `mobile/docs/11-NamingConvention.md`
