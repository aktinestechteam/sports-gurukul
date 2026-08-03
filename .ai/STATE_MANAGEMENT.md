# STATE_MANAGEMENT

Status: **Adopted** - Owner: Chief Software Architect

## 1. Rule

**Riverpod is the only state management / DI solution.**

- No `provider` package.
- No Bloc / flutter_bloc.
- No GetX.
- No setState-only architecture (local ephemeral state is fine).
- No InheritedWidget hacking, no service locators.

Current package: `flutter_riverpod ^3.4.2`.

## 2. Provider types (Riverpod 3)

| Need | Type |
| --- | --- |
| Constant / injectable dependency | `Provider<T>` |
| Synchronous derived value | `Provider<T>` (or `Provider` + selector) |
| Asynchronous one-shot | `FutureProvider<T>` / `AsyncNotifierProvider<T, State>` |
| Mutable app state (theme, session) | `NotifierProvider<Notifier, State>` |
| Stream-based state (connectivity) | `StreamProvider<T>` |
| Per-route/feature state | `Provider`/`Notifier` scoped to the feature module |

Prefer `Notifier` over the older `StateNotifier`/`ChangeNotifier` styles.
Keep providers deterministic; side effects belong in use cases/services, not
in `build()`.

## 3. Naming conventions

- Providers: `<thing>Provider` - e.g. `connectivityServiceProvider`,
  `preferenceStorageProvider`, `secureStorageProvider`.
- Notifiers: `<Thing>Notifier` - e.g. `ThemeModeNotifier`.
- Files: providers live in their owning module
  (`presentation/providers/`, or beside the service in `core/`), one file per
  concern. A provider file may define its notifier class.

## 4. Wiring & composition

- Providers ARE the dependency container (`app/dependency_container.dart`).
- Production composition: `app/bootstrap.dart` + `ProviderScope`.
- Tests override providers at the `ProviderScope` (Riverpod 3 keeps
  `Override` internal - pass override lists at the call site).
- Repository -> datasource -> use case -> view-model provider chains are built
  bottom-up; pages consume the topmost provider they need.

## 5. Codegen note

Riverpod codegen (`riverpod_generator`/`riverpod_annotation`) is **deferred**
until its analyzer requirement (^12/^13) is compatible with `freezed 3.2.5`
(analyzer < 11). Hand-written providers are the current standard. Revisit per
`DECISIONS.md`.

## 6. Reference

- `docs/mobile/09-Implementation/03-Riverpod-Architecture.md`
- `docs/mobile/09-Implementation/08-State-Management-Standards.md`
- Theme example: `lib/app/theme/material_theme/theme_mode_provider.dart`
