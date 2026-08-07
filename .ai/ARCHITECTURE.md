# ARCHITECTURE

Status: **Adopted** - Owner: Chief Software Architect

Authoritative detail lives in `docs/mobile/09-Implementation/` and
`mobile/docs/`. This file is the reference an AI needs before writing code.

## 1. Layers

The app uses **Clean Architecture + Feature First**. Each feature owns four
layers; shared concerns live in `core/` and `shared/`.

```
lib/
+-- core/            # Non-UI shared infrastructure (network, storage, db, logging)
+-- shared/          # UI shared widgets, models, utilities
+-- features/
|   \-- <feature>/
|       +-- presentation/     Pages, widgets, providers (Riverpod)
|       +-- application/      Use cases, services (orchestration)
|       +-- domain/           Entities, repository interfaces, value objects, failures
|       \-- infrastructure/   Datasources, DTOs, mappers, repository implementations
\-- app/             # App shell: bootstrap, router, theme, DI container
```

## 2. Dependency Flow

```
Presentation -> Application -> Domain <- Infrastructure
```

- **Domain** is the innermost, dependency-free core (no Flutter/HTTP/JSON).
- **Application** orchestrates use cases; depends only on domain abstractions.
- **Infrastructure** implements the repository interfaces and knows the
  outside world (Dio, Drift, storage).
- **Presentation** consumes providers; it never constructs repositories,
  never calls APIs, never touches the database.

## 3. Dependency Injection

- **Riverpod providers are the dependency container.** No global service
  locator, no `static` singletons for services.
- Providers live in their owning module (`presentation/providers/`, or next
  to the service in `core/`).
- Composition happens in `app/dependency_container.dart` and
  `app/bootstrap.dart`; tests override providers at the `ProviderScope`.
- Concrete wiring facts: `ApiClient.create()` builds Dio with the interceptor
  chain; `AppDatabase` is the Drift database; storage facades wrap
  flutter_secure_storage / shared_preferences.

## 4. Folder Structure

See `FOLDER_STRUCTURE.md` for the exact tree and placement rules.

## 5. Feature Development

1. Define the domain entity + repository interface in `domain/`.
2. Implement DTO + mapper + repository in `infrastructure/`.
3. Write use cases in `application/`.
4. Expose providers and build pages in `presentation/`.
5. Route is registered in `app/router/` (names + paths centralized) and
   guarded if role-specific.
6. All user-facing strings via l10n ARB; all styling via design tokens.

## 6. Core Components (existing)

| Concern | File / Type | Notes |
| --- | --- | --- |
| Logging | `AppLogger` (`core/logging/app_logger.dart`) | `print()` banned |
| HTTP | `ApiClient` + interceptors (`core/network`, `core/interceptors`) | RequestId->Auth->Logging->Retry |
| Errors | `mapNetworkError` -> `NetworkErrorKind` | boundary mapping |
| Database | `AppDatabase` (`core/database/app_database.dart`) | Drift, schemaVersion 1 |
| Offline | `OfflineQueue` (`core/offline/offline_queue.dart`) | outbox scaffold |
| Secure storage | `SecureStorage` (`core/storage/`) | secrets only |
| Preferences | `PreferenceStorage` (`core/storage/`) | non-sensitive only |
| Connectivity | `ConnectivityService` (`core/connectivity/`) | online/offline stream |
| App metadata | `AppInfo`, `DeviceInfoService` (`core/info/`) | package/device info |
| Unique ids | `UniqueId` (`core/utils/`) | uuid v4 |
| Config | `app/config/` (`environment.dart`, `app_config.dart`) | env-driven config |

## 7. Shared Components

Shared UI (widgets, tokens, `shared/models/`) is promoted from features when
the same behaviour is needed twice. Nothing in `core/` or `shared/` may
depend on a feature.

## 8. Reference

- Clean Architecture detail: `docs/mobile/09-Implementation/02-Clean-Architecture.md`
- Riverpod architecture: `docs/mobile/09-Implementation/03-Riverpod-Architecture.md`
- Repository pattern: `docs/mobile/09-Implementation/05-Repository-Pattern.md`
- Sprint docs: `mobile/docs/01-Architecture-Overview.md`, `mobile/docs/02-Folder-Structure.md`
