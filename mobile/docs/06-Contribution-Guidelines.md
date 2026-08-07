# Contribution Guidelines

Status: **Approved baseline** · Sprint 0 (P001)

## 1. Coding Standards

Reference: `docs/mobile/09-Implementation/09-Coding-Standards.md`.

- **Null safety, `final` by default, `const` constructors** where possible.
- **Named parameters**; trailing commas on multi-line widgets.
- **Strong typing** — no `dynamic`, no unjustified `!`.
- Method length < 40 lines; class length < 300 lines; cyclomatic
  complexity < 10.
- **Naming:** classes `PascalCase`, files `snake_case`, variables/consts
  `camelCase`, private members underscore-prefixed, enums `PascalCase`.
  Screens `XxxPage`, widgets `XxxCard`, providers `XxxProvider`,
  repositories `XxxRepository`, models `XxxDto`, entities `Xxx`,
  use cases `VerbXxx`.
- **Import order:** Dart SDK → Flutter SDK → third-party → internal →
  relative; blank line between groups.
- **No hardcoded user-visible strings** — use `context.l10n.*`.
- **No hardcoded colors/values** — use design tokens (`AppColors`,
  `AppSpacing`, `AppRadius`, …).
- **No hardcoded route strings** — use `RoutePaths` / `RouteNames`.
- **No `print`** — use the structured logger when available.
- **No unhandled exceptions to the UI** — features return `Result<T, Failure>`.
- **Layering:** Presentation never calls APIs/databases; DTOs never leave
  infrastructure; domain has no Flutter dependency; features never reach
  into other features.

## 2. Commit Conventions

Conventional commits: `type(scope): summary`

```
feat(mobile-auth): add OTP verification flow
fix(mobile-dashboard): correct greeting on cold start
refactor(mobile-core): extract api client interceptors
docs(mobile): update folder structure
test(mobile-auth): cover login validation
```

Types: `feat`, `fix`, `refactor`, `docs`, `test`, `chore`, `perf`, `ci`,
`build`.

Never commit secrets, `.env` files, or generated binaries. Inspect staged
changes before committing (`git status`, `git diff --cached`).

## 3. Pull Request Process

1. Branch from the current mobile baseline: `feature/<descriptive-name>`.
2. Keep changes scoped to one task (one P-prompt per PR).
3. Run the quality gate (`./scripts/check.sh` or the manual equivalent)
   locally — it must pass.
4. Open the PR with a summary referencing the P-prompt and the docs it
   satisfies.
5. Request review; the reviewer checks architecture compliance, SOLID,
   performance and security.
6. Squash-merge after approval and green CI.

## 4. Review Checklist

- [ ] Scope matches the task (no gold-plating)
- [ ] Zero analyzer warnings
- [ ] All tests pass
- [ ] New code formatted (`dart format`)
- [ ] Follows the layer rules and folder structure
- [ ] Uses l10n and design tokens (no hardcoded strings/colors)
- [ ] No dead code, duplicate code, or unused imports
- [ ] No new secrets committed
- [ ] Documentation updated if behavior/structure changed

## 5. Definition of Done

See [Development Workflow](03-Development-Workflow.md). A task is complete
only when it builds, runs, analyzes clean, tests green, and is documented.
