# Coding Standards

Status: **Adopted** · Owner: Mobile Architecture Team

## 1. Language & Formatting

- Dart code, formatted with `dart format`. CI enforces
  `dart format --set-exit-if-changed`.
- `analysis_options.yaml` includes `very_good_analysis` (strict lint set;
  see `docs/09-EngineeringStandards.md` for exclusions and deviations).
  Fix warnings; do not suppress them with ignores.
- No trailing whitespace; one blank line between logical blocks; no
  unused imports (autofix with `dart fix --apply`).

## 2. Architecture Rules

- **Presentation never calls APIs or databases directly** — always through
  a provider → use case/service → repository.
- **Domain has no Flutter, HTTP or JSON dependency.**
- **DTOs never leave the infrastructure layer** — map to domain entities at
  the boundary.
- **Nothing in `core/` or `shared/` depends on a feature.**
- **No cross-feature imports.** If two features need the same behaviour,
  promote it to `core/` (non-UI) or `shared/` (UI).
- **No global singletons** — dependencies are constructor-injected through
  Riverpod providers.

## 3. Naming & Structure

- Dart files: `snake_case.dart`. Widget classes: `PascalCase`.
- Private members prefixed with `_`; never expose mutable state publicly.
- Abstract repository interfaces live in `domain/repositories/`; concrete
  implementations in `infrastructure/repositories/` as `XxxRepositoryImpl`.
- Datasource/DTO models live in `infrastructure/models/` (or `datasources/`).
- Constants: `abstract final class` with `static const` members
  (`AppColors`, `AppSpacing`). Never sprinkle magic numbers in widgets.

## 4. State Management (Riverpod)

- Providers are file-local by default; export only what pages consume.
- One provider per repository/service where sensible; compose higher-level
  providers for view models.
- Keep providers deterministic; side effects belong in use cases/services.
- Do not store ephemeral widget state in providers unless it must survive
  rebuilds or be shared.

## 5. Localization

- **No hardcoded user-facing strings.** All strings come from `l10n/arb/`
  and the generated `AppLocalizations`.
- New strings require the English ARB plus every supported locale
  (en/hi/mr). Run `flutter gen-l10n` after editing ARB files.

## 6. Theming

- **No hardcoded colors, spacing, radius, or typography values in
  widgets.** Use the design tokens (`AppColors`, `AppSpacing`,
  `AppRadius`, `AppTypography`, ...).
- Theme variants are produced from the single seed `Color(0xFF006DFF)`
  via `ColorScheme.fromSeed`.

## 7. Errors & Resilience

- Features return `Result<T, Failure>`; never surface raw exceptions to
  the UI.
- `Failure` types live in the feature `domain/` layer; map infrastructure
  errors to domain failures in the repository implementation.
- Always handle the error and empty states of async screens.

## 8. Security

- Never commit secrets, API keys, or tokens. Inject them through
  environment configuration and secure storage.
- HTTPS only; sensitive data never logged. PII minimised in logs.

## 9. Testing

- Unit tests for pure logic in `test/unit/`; widget tests in `test/widget/`;
  feature tests in `test/features/`.
- Shared fixtures in `test/fixtures/`, mocks in `test/mocks/`, helpers in
  `test/helpers/`.
- New logic ships with tests; CI blocks on failures (`flutter test`).
