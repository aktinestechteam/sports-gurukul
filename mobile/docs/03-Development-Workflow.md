# Development Workflow

Status: **Approved baseline** · Sprint 0 (P001)

## 1. Methodology

This repository follows **Scrum** at the sprint level and **GitFlow**-style
branching at the repository level.

- **Sprint 0** bootstrapped the foundation (P001).
- Subsequent sprints deliver features: authentication, dashboard, roles,
  booking, tournaments, notifications, AI, settings.
- Every task is executed as a self-contained P-code prompt with its own
  definition of done.

## 2. Branching

| Branch | Purpose |
| ------ | ------- |
| `main` | Production-ready, releaseable |
| `mobile_project_foundation` | Mobile foundation line (current) |
| `feature/*` | Feature work (e.g. `feature/mobile-auth`) |
| `fix/*` | Bug fixes |

Work only on branches; never commit directly to `main`.

## 3. Task Execution Order

1. **Analyze** — review requirements against the approved docs
   (`docs/mobile/`, `docs/`). Never redesign approved APIs.
2. **Plan** — list files, dependencies, build order, expected output.
3. **Implement incrementally** — verify compilation after each major change.
4. **Verify** — `flutter pub get`, `flutter analyze`, `flutter test`,
   `flutter run`. Fix and re-run until green.
5. **Review** — architecture + QA pass (SOLID, naming, M3, performance,
   security, no dead/duplicate code, no hardcoded strings/colors).
6. **Report** — sprint report: summary, files, dependencies, commands,
   issues, risks, debt, next task, definition of done.

## 4. Quality Gate

The `scripts/check.sh` script (or the equivalent manual commands) is the
entry gate. A change is not complete until:

- `dart format` reports no diffs,
- `flutter analyze` reports **zero** warnings/errors,
- `flutter test` is fully green,
- integration smoke test passes on a device/emulator.

## 5. Definition of Done

- [ ] Scope matches the P-prompt exactly (no out-of-scope implementation)
- [ ] Builds successfully
- [ ] Runs successfully (splash → dashboard)
- [ ] Zero analyzer warnings
- [ ] No failing tests
- [ ] Folder structure completed and documented
- [ ] Documentation updated
- [ ] Sprint report produced

## 6. Do Not

- Rewrite the approved architecture.
- Duplicate code or re-implement existing utilities.
- Skip verification or review.
- Implement functionality outside the task scope.
- Connect to or guess backend contracts without the Swagger/OpenAPI spec.
- Mark a task complete until verification passes.
