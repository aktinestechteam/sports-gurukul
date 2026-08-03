# Architecture Overview

Status: **Approved baseline** · Owner: Mobile Architecture Team
Reference: `docs/mobile/09-Implementation/01-Flutter-Project-Architecture.md`
and `docs/mobile/09-Implementation/02-Clean-Architecture.md`.

## 1. Guiding Principles

The application is built on **Clean Architecture**, **Feature First**,
**SOLID**, **DRY**, **KISS** and **YAGNI**. It is **offline-first ready**
and **dependency-injection ready** (Riverpod).

- **Presentation never calls APIs or databases directly.**
- **Domain has no Flutter, HTTP or JSON dependency.**
- **DTOs never leave the infrastructure layer.**
- **Nothing in `core/` depends on a feature; shared widgets are
  business-independent.**
- **No hardcoded user-facing strings or colors** — everything comes from
  `l10n/` and the design tokens.
- **No global singletons** — dependencies are constructor-injected through
  Riverpod providers.

## 2. Layers

```
lib/
├── app/        Application bootstrap: startup, routing, theme, config
├── core/       Reusable infrastructure (business-independent)
├── shared/     Reusable UI components (business-independent)
├── features/   Feature modules, each with 4 layers
├── l10n/       Localization sources (ARB) + generated classes
└── assets/     Images, icons, animations, fonts
```

### Feature module (Feature First)

Each feature is self-contained:

```
features/<feature>/
├── presentation/   Pages, widgets, providers, controllers
├── application/    Use cases, commands, queries, services (orchestration)
├── domain/         Entities, repository interfaces, value objects, failures
└── infrastructure/ API/datasource/repository implementations, models, mappers
```

Dependency flow is strictly inward:
`Presentation → Application → Domain → Infrastructure`.

## 3. Cross-Cutting Concerns

| Concern | Approach |
| ------- | -------- |
| Dependency Injection | Riverpod 3.x `ProviderScope` at the root; providers compose repositories/datasources |
| Navigation | go_router; centralized `route_names.dart` / `route_paths.dart`; route guards and role-based shells scaffolded |
| Theming | Material 3; single seed color `Color(0xFF006DFF)`; `ColorScheme.fromSeed` for light + dark |
| Design tokens | `AppColors`, `AppSpacing`, `AppRadius`, `AppTypography`, `AppElevation`, `AppShadow`, `AppAnimation` |
| Localization | `flutter_localizations` + `gen_l10n`; `app_en.arb`, `app_hi.arb`, `app_mr.arb`; fallback: en |
| Offline first | Repository pattern isolates sync; Drift/SQLite planned for the database sprint |
| Errors | Features return `Result<T, Failure>`; never throw unhandled exceptions to the UI |
| Security | Never commit secrets; HTTPS only; sensitive data never logged (policy in coding standards) |

## 4. Startup Sequence

```
main() → AppBootstrap.initialize() → runApp(ProviderScope → SportsGurukulApp)
       → SplashPage → DashboardPage
```

`AppBootstrap` mirrors the approved bootstrap order (Logging → Secure
Storage → Database → API → Authentication → Analytics → Notifications →
Synchronization). Each step is a Sprint 0 placeholder, wired incrementally.

## 5. Sprint 0 Scope

Created in P001: project scaffold, folder structure, Material 3 theming,
localization (en/hi/mr), routing skeleton, placeholder dashboard, test and
CI tooling, and documentation.

**Explicitly deferred:** authentication, business features, API/DTO/repository
implementation, local database, role-based navigation, analytics.

## 6. Known Trade-offs

- `intl` pinned to `0.20.2` by `flutter_localizations` (SDK constraint).
- Riverpod 2.x → 3.x migration completed in P001 (API surface used is
  stable).
- Inter brand font will be bundled when the font asset is delivered;
  Roboto is used until then.
- `mobile/pubspec.lock` is gitignored by the repository root — recommended
  to be committed for an application (see Technical Debt).
