# DESIGN_SYSTEM

Status: **Adopted** - Owner: Chief Software Architect

## 1. Core rule

**Material 3 only. Nothing hardcoded.** No raw colors, typography, spacing,
or radius values in widgets - everything flows through the theme and design
tokens.

- Theme seed: `Color(0xFF006DFF)` -> `ColorScheme.fromSeed` (Material 3).
- Tokens live in `abstract final class` holders with `static const`:
  - `AppColors` (`app/theme/colors/app_colors.dart`) - semantic colors
    (`primary500`, surface, error, ...).
  - `AppSpacing` (`app/theme/spacing/app_spacing.dart`) - spacing scale
    (`xs`..`xxxl`).
  - Radius/typography tokens to be added with the design-system sprint
    (same pattern).
- Theme construction: `app/theme/app_theme.dart` (light/dark variants from
  the seed); `ThemeMode` handled by the theme-mode notifier.

## 2. Usage rules

- Colors: `Theme.of(context).colorScheme.*` for dynamic surfaces;
  `AppColors.*` for fixed brand semantics.
- Spacing: `AppSpacing.*` - never bare `EdgeInsets.all(8)`.
- Typography: `Theme.of(context).textTheme.*` - never bare `fontSize:`.
- Shape: theme `CardTheme`/`shape` defaults - never bare `BorderRadius`.
- Icons: Material icon set; avoid third-party icon packages until approved.
- Dark mode: use theme variants; never conditional hex colors in widgets.

## 3. Widget consistency

- Reusable UI (buttons, inputs, cards) is promoted to `shared/widgets/`
  and themed, so app-wide changes land in one place.
- Keep semantic color meaning consistent (error, success, warning, info).

## 4. Reference

- `docs/mobile/01-Design-System.md` (authoritative product design spec)
- `docs/mobile/00-Mobile-App-Vision.md`
- Implementation: `mobile/docs/`, `mobile/lib/app/theme/`
