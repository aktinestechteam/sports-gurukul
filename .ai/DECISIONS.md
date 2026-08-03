# DECISIONS (Architecture Decision Record)

Status: **Living** - Owner: Chief Software Architect

Every significant architectural decision is recorded here as an ADR. New
decisions are appended; entries are never silently edited (superseded entries
get an update note). Package-level choices are logged in
`mobile/docs/13-PackageDecisionLog.md`.

## ADR template

```markdown
## ADR-NNN - <Title>

- **Date:** YYYY-MM-DD
- **Status:** Proposed | Accepted | Deprecated | Superseded by ADR-NNN
- **Context:** <problem, constraints, forces>
- **Decision:** <what we chose>
- **Consequences:** <positive + negative; follow-up work>
- **Alternatives:** <options considered and why rejected>
```

## Recorded decisions

### ADR-001 - Clean Architecture + Feature First
- **Status:** Accepted
- **Context:** Multi-role app (Athlete/Parent/Coach/Academy/Super Admin) in
  one codebase; long-lived; many AI sessions and contributors.
- **Decision:** Clean Architecture (Presentation/Application/Domain/
  Infrastructure per feature) with `core/` (non-UI) and `shared/` (UI)
  shared components. Dependency direction is fixed; domain is dependency-free.
- **Consequences:** Disciplined layering; DTOs never leak; promotion rule for
  shared code.
- **Alternatives:** single-layer MVC (state in widgets - rejected), modular
  micro-apps (overhead).

### ADR-002 - Riverpod-only state management & DI
- **Status:** Accepted
- **Context:** Need DI + state without a global service locator.
- **Decision:** Riverpod 3.x is the only state management/DI solution.
  Providers are the dependency container; composition in
  `app/dependency_container.dart` + `ProviderScope`.
- **Consequences:** No Provider/Bloc/GetX. Riverpod codegen deferred (see
  ADR-004).
- **Alternatives:** Bloc (verbosity, rejected), GetX (anti-pattern, rejected).

### ADR-003 - Dio-only networking with a fixed interceptor chain
- **Status:** Accepted
- **Context:** Completed backend, need retries, auth injection, tracing.
- **Decision:** Dio ^5 via `ApiClient.create()`; chain
  RequestId -> Auth -> Logging -> Retry; errors via `mapNetworkError` ->
  `NetworkErrorKind`. Retries centralized, never per-call.
- **Consequences:** Consistent tracing (`X-Request-Id`), one place for
  auth/retry/logging.
- **Alternatives:** `http` (no interceptors - rejected).

### ADR-004 - Riverpod codegen deferred (analyzer conflict)
- **Status:** Accepted (revisit when resolvable)
- **Context:** `riverpod_generator` needs analyzer ^12/^13; `custom_lint`
  needs ^8; `freezed 3.2.5` needs >= 9 < 11. No single analyzer satisfies all.
- **Decision:** Hand-written Riverpod 3 providers; revisit codegen when the
  analyzer constraint clears.
- **Consequences:** Slightly more boilerplate now; stable build.
- **Alternatives:** pre-release analyzer hacks (rejected).

### ADR-005 - Drift-only offline-first database
- **Status:** Accepted
- **Context:** Offline-first requirement; need typed, migration-safe schema.
- **Decision:** Drift ^2.34 + `drift_flutter` + `drift_dev`. `AppDatabase`
  schemaVersion 1, migration scaffold, `PRAGMA foreign_keys = ON`.
- **Consequences:** Typed queries, safe migrations; DAO pattern in P005+.
- **Alternatives:** sqflite (no safety), isar/hive (less mature migrations).

### ADR-006 - Storage split: secure vs prefs
- **Status:** Accepted
- **Context:** Secrets must be encrypted; prefs must be light.
- **Decision:** Secrets -> `SecureStorage` (flutter_secure_storage/keychain);
  non-sensitive -> `PreferenceStorage` (shared_preferences). Never share a key
  namespace; never cross over.
- **Consequences:** Clear security boundary; easy testing via injected backends.

### ADR-007 - Strict linting via very_good_analysis
- **Status:** Accepted
- **Context:** Multiple AI contributors; need enforced consistency.
- **Decision:** very_good_analysis ^10.3.0 replaces flutter_lints. Zero-issue
  `flutter analyze` is a gate. Generated code excluded; two documented
  deviations (`public_member_api_docs: false`, `invalid_annotation_target:
  ignore`).
- **Consequences:** Stronger defaults, occasional `// ignore: <lint>` with reason.

### ADR-008 - Built-in goldens (golden_toolkit discontinued)
- **Status:** Accepted
- **Context:** `golden_toolkit` unmaintained.
- **Decision:** Use `matchesGoldenFile`; goldens platform-specific (Ahem),
  regenerate via `--update-goldens`.

### ADR-009 - Localization: gen_l10n (en, hi, mr)
- **Status:** Accepted
- **Context:** Multi-language product; ARB is the single source.
- **Decision:** flutter_localizations + gen_l10n; all user-facing strings from
  ARB; generated code excluded from analysis.

### ADR-010 - Offline verification target is web (Windows dev)
- **Status:** Accepted
- **Context:** Local dev is Windows without an Android SDK.
- **Decision:** Verify with `flutter build web` locally; Android/iOS are CI
  release targets.
- **Consequences:** Web must compile cleanly; keep mobile-specific APIs
  behind guards if ever introduced.
