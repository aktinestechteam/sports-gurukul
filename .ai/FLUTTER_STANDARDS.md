# FLUTTER_STANDARDS

Status: **Adopted** - Owner: Chief Software Architect

Flutter-specific conventions. Combine with `CODING_STANDARDS.md`,
`DESIGN_SYSTEM.md`, and `STATE_MANAGEMENT.md`.

## 1. App shell

- Single `MaterialApp.router` (`app/app.dart`) using go_router.
- `runApp` flows through `app/bootstrap.dart`; composition is Riverpod-based.
- Splash page (`app/bootstrap/splash_page.dart`) owns startup/init flow and
  then navigates to the first destination.

## 2. Routing

- go_router ^17 only. Routes, names and paths are centralized:
  `app/router/app_router.dart`, `route_names.dart`, `route_paths.dart`.
- Role-based access via `app/router/guards/route_guards.dart` (scaffolded;
  becomes functional with auth in P005+).
- Pages are lazy route targets; keep the route table small and declarative.

## 3. Widgets & state

- Stateless first; introduce state only where needed.
- Ephemeral UI state stays in the widget; shared/async state goes to Riverpod
  providers (see `STATE_MANAGEMENT.md`).
- No `setState` inside `build`; no network/DB work in `build`.
- Widgets are small and single-purpose; extract shared widgets to
  `shared/widgets/`.
- Use `const` constructors everywhere possible.

## 4. Async & streams

- Prefer async/await over raw futures; never ignore errors silently.
- Streams are handled with explicit error paths; subscriptions are cancelled.
- Loading/error/empty states are required on every async screen.

## 5. Localization (l10n)

- All user-facing strings in ARB: `lib/l10n/arb/app_{en,hi,mr}.arb`.
- Access via generated `AppLocalizations.of(context)`.
- After editing ARB: `flutter gen-l10n`, then re-run the verification gates.
- Never embed raw text in widgets.

## 6. Theming

- Material 3, seeded from `Color(0xFF006DFF)` via `ColorScheme.fromSeed`.
- Use tokens: `AppColors`, `AppSpacing` (radius/typography tokens land with
  the design system extension). Never hardcode values in widgets.
- Theme variants and `ThemeMode` flow through
  `app/theme/` and the theme-mode notifier.

## 7. Models & codegen

- Domain entities and DTOs are immutable `freezed` classes.
- `dart run build_runner build` regenerates `.g.dart`/`.freezed.dart`;
  commit the output, never hand-edit it.

## 8. Platform & environment

- Environment selection via `app/config/environment.dart` +
  `app_config.dart`. Base URL resolution lands with env configuration.
- Web is the local offline verification target; no mobile-specific APIs are
  called on web paths (use conditional imports if ever required).

## 9. Anti-patterns (never)

- `print()`/`debugPrint` for logging -> `AppLogger`.
- Raw `BuildContext` passed to async gaps without guard checks when needed.
- Global mutable state, service locators, `InheritedWidget` hacks.
- Blocking the UI thread; heavy work stays in async/isolates.
- Hardcoded strings, colors, spacing, or magic numbers in widgets.
