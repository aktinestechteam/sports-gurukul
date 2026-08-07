# UI_GUIDELINES

Status: **Adopted** - Owner: Chief Software Architect

Applies to every screen. Pairs with `DESIGN_SYSTEM.md`.

## 1. Layout & responsiveness

- Mobile-first; layouts adapt across phone sizes, tablets, and (web) wide
  breakpoints using `LayoutBuilder`/`MediaQuery` or adaptive widgets.
- No fixed-pixel layouts; use the spacing/typography tokens and flexible
  layouts (`Flexible`, `Expanded`, `Wrap`, `GridView`).
- Respect safe areas (`SafeArea`) and system insets.
- Web target is a first-class verification target - no dead-end mobile-only
  assumptions (conditional imports only when strictly required).

## 2. States

Every async screen must handle:
- **Loading** - progress indicator or skeleton.
- **Error** - human message, retry action, typed failure from the feature.
- **Empty** - guidance + primary action.
- **Data** - fully populated.

Never show raw exceptions or stack traces to users.

## 3. Accessibility (a11y)

- Semantic labels on icon-only controls; `Tooltip` where appropriate.
- Sufficient contrast against both light and dark themes (test on both).
- Touch targets >= 48x48 logical pixels.
- Text scaling respected (no `maxLines`+fixed-height clipping of content).
- Don't rely on color alone to convey state.

## 4. Localization

- Every user-facing string from ARB (`en`, `hi`, `mr`); run
  `flutter gen-l10n` after edits.
- No hardcoded text, even for placeholders or empty states.
- Keep concatenation minimal; use placeholders/pluralization in ARB.

## 5. Navigation & flows

- Routes via go_router (see `FLUTTER_STANDARDS.md` Section 2).
- Loading/success/error transitions are explicit; never silently navigate.
- Back behaviour is predictable (back stack, guarded pages).

## 6. Performance in UI

- `const` constructors; `RepaintBoundary` around heavy subtrees; avoid
  rebuilding lists (`itemBuilder`, keys); use `ListView.builder` not
  `Column` of thousands. See `PERFORMANCE.md`.

## 7. Review

UI changes are reviewed against `REVIEW_CHECKLIST.md` (accessibility,
localization, responsiveness, performance).
