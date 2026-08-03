# Git Workflow

Status: **Adopted** · Owner: Mobile Architecture Team

## 1. Branching

- `main` is protected; all work happens on branches.
- Branch prefixes: `feature/<slug>`, `fix/<slug>`, `chore/<slug>`,
  `docs/<slug>`, `refactor/<slug>`.
- One branch per deliverable; keep the change scoped.

## 2. Commits

- Conventional Commits:
  `feat:`, `fix:`, `docs:`, `chore:`, `refactor:`, `test:`, `perf:`,
  `build:`, `ci:`.
- One logical change per commit; never commit secrets, keys, or generated
  noise.
- `pubspec.lock` is tracked — do not gitignore it.
- Generated files (`.g.dart`, `.freezed.dart`, drift output) are committed
  and regenerated via `dart run build_runner build`.

## 3. Pull Requests

- PRs target `main`; CI must pass format, analyze and test.
- Reviewer checklist:
  - No secrets or debug `print()`; no sensitive logging.
  - No hardcoded user-facing strings, colors or spacing.
  - Tests included for new behaviour.
  - New dependencies referenced in `docs/13-PackageDecisionLog.md`.
  - No cross-feature imports.

## 4. Release

- Version bumps follow semantic versioning in `pubspec.yaml`.
- Tag releases and update the changelog.
