# FOLDER_STRUCTURE

Status: **Adopted** - Owner: Chief Software Architect

This is the authoritative `lib/` and `test/` layout for the Flutter app.
Authoritative prose version: `mobile/docs/02-Folder-Structure.md`.

## 1. Source tree (`mobile/lib/`)

```
lib/
+-- main.dart                        # Entry point
+-- app/
|   +-- app.dart                     # SportsGurukulApp (MaterialApp.router)
|   +-- bootstrap.dart               # Composition root / runApp
|   +-- bootstrap/
|   |   \-- splash_page.dart         # Branded splash + init flow
|   +-- config/
|   |   +-- app_config.dart          # Config model
|   |   \-- environment.dart         # Env selection
|   +-- dependency_container.dart    # Composition notes; providers live in modules
|   +-- router/
|   |   +-- app_router.dart          # GoRouter + route table
|   |   +-- route_names.dart         # RouteNames constants
|   |   +-- route_paths.dart         # RoutePaths constants
|   |   +-- guards/route_guards.dart # Role guards (scaffold)
|   |   \-- navigation/navigation_service.dart
|   \-- theme/
|       +-- app_theme.dart           # Material 3 theme from seed
|       +-- material_theme/theme_mode_provider.dart
|       +-- colors/app_colors.dart   # Color tokens
|       \-- spacing/app_spacing.dart # Spacing tokens
+-- core/                            # Non-UI shared infrastructure
|   +-- connectivity/connectivity_service.dart
|   +-- database/app_database.dart   # Drift (+ generated .g.dart)
|   +-- extensions/                  # Shared extensions
|   +-- info/
|   |   +-- app_info.dart            # package_info_plus
|   |   \-- device_info.dart         # device_info_plus
|   +-- interceptors/
|   |   +-- auth_interceptor.dart    # Placeholder (token injection P005+)
|   |   +-- logging_interceptor.dart # Status/URI only
|   |   +-- request_id_interceptor.dart
|   |   \-- retry_interceptor.dart   # Bounded retries
|   +-- logging/app_logger.dart      # Logger facade
|   +-- network/
|   |   +-- api_client.dart          # ApiClient.create()
|   |   +-- error_mapper.dart        # mapNetworkError -> NetworkErrorKind
|   |   \-- network_config.dart      # Timeouts, retries, header names
|   +-- offline/offline_queue.dart   # Outbox scaffold
|   +-- storage/
|   |   +-- preference_storage.dart  # shared_preferences (non-sensitive)
|   |   +-- secure_storage.dart      # flutter_secure_storage (secrets)
|   |   \-- storage_providers.dart   # Riverpod providers
|   \-- utils/unique_id.dart         # uuid v4
+-- features/
|   +-- dashboard/presentation/pages/dashboard_page.dart   # Placeholder
|   \-- <feature>/{presentation,application,domain,infrastructure}/
+-- l10n/
|   +-- arb/                         # app_en.arb, app_hi.arb, app_mr.arb
|   \-- generated/                   # AppLocalizations (do not hand-edit)
\-- shared/
    +-- models/                      # Reusable models (sample_model.dart P003; replaced in P005)
    \-- widgets/                     # Shared UI widgets
```

## 2. Test tree (`mobile/test/`)

```
test/
+-- unit/          # Pure logic: mappers, services, models, facades
+-- widget/        # Widget tests + goldens/
|   \-- goldens/   # Golden images (regenerate via --update-goldens)
+-- integration/   # Full-flow integration tests
+-- fixtures/      # Shared test data (files)
+-- helpers/       # Test utilities (pump helpers, etc.)
\-- mocks/         # mocktail mocks (e.g. flutter_secure_storage_mock.dart)
```

## 3. Placement Rules

- A widget/page that is used by exactly one feature -> `features/<feature>/presentation/`.
- Used by two or more features -> `shared/widgets/`.
- Non-UI logic used by two or more features -> `core/`.
- **No cross-feature imports.** Promote instead.
- Constants holders are `abstract final class` with `static const`
  (`AppColors`, `AppSpacing`, `RouteNames`, `RoutePaths`).
- Generated files (`.g.dart`, `.freezed.dart`) live beside their source and
  are committed; never hand-edited.

## 4. Dependency constraints

- `core/` and `shared/` never import `features/`.
- `app/` may import `core/`, `shared/`, `features/` (router needs pages).
- `features/*` import only `core/`, `shared/`, and their own layers.
