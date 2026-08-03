# Naming Conventions

Status: **Adopted** · Owner: Mobile Architecture Team

## 1. Files & Folders

- Dart files: `snake_case.dart`.
- Widget classes: `PascalCase` (e.g. `dashboard_page.dart` → `DashboardPage`).
- Feature layout: `features/<feature>/{presentation,application,domain,infrastructure}`.
- Folder names are singular nouns (`page`, `widget`, `usecase`, `entity`).

## 2. Types

- Classes, enums, mixins: `PascalCase`.
- Constants holders: `abstract final class` with `static const` members
  (`AppColors`, `AppSpacing`).
- Facades that wrap a package: package name in PascalCase (`AppLogger`,
  `ConnectivityService`, `UniqueId`, `AppInfo`).
- Extensions: `<ExtendedType>X` (`StringX`, `DateTimeX`). One extension per
  file named `<name>_x.dart` in `lib/core/extensions/`.

## 3. Members

- Methods, fields, getters, setters: `camelCase`.
- Private members are prefixed `_`; mutable state is never public.
- Boolean getters/fields use `isXxx` / `hasXxx`.
- Boolean setters are written as a `set` property (`set active(bool value)`),
  not `setActive(...)`.
- Lint enforcers: `use_setters_to_change_properties`,
  `avoid_positional_boolean_parameters`.

## 4. Layer Conventions

- Use cases: `VerbNoun` (`LoginUser`, `FetchProfile`). One action, one class.
- Repository interfaces: `XxxRepository` in feature `domain/repositories/`.
- Repository implementations: `XxxRepositoryImpl` in
  `infrastructure/repositories/`.
- DTOs: `XxxDto` in `infrastructure/models/`; never leave infrastructure.
- Domain entities: plain nouns, no suffix (`Profile`).
- Failures: `XxxFailure` in feature `domain/failures/`.

## 5. Routing & DI

- Route names: `RouteNames.x`; paths: `RoutePaths.x` — centralized in
  `app/router/`.
- Providers: `<thing>Provider` (e.g. `connectivityServiceProvider`).
- Notifiers: `<Thing>Notifier` (e.g. `ThemeModeNotifier`).
- A provider file may define its notifier; keep providers file-local and
  export only what pages consume.

## 6. Constants & Strings

- ARB keys: `snake_case`; generated accessors: `camelCase`.
- Design tokens use semantic names (`primary500`, `xxxl`); never raw hex
  values or magic numbers in widgets.
