# GIT_WORKFLOW

Status: **Adopted** - Owner: Chief Software Architect

Also mirrored in `mobile/docs/12-GitWorkflow.md`.

## 1. Branch strategy

- `main` is protected. All work happens on branches.
- Prefixes:
  - `feature/<slug>` - new functionality
  - `fix/<slug>` - bug fixes
  - `chore/<slug>` - maintenance, tooling
  - `docs/<slug>` - documentation
  - `refactor/<slug>` - non-functional restructuring
  - `hotfix/<slug>` - urgent production fixes (see Section 4)
- One branch per deliverable; keep changes scoped. Small, reviewable diffs.

## 2. Commit conventions

- Conventional Commits: `feat:`, `fix:`, `docs:`, `chore:`, `refactor:`,
  `test:`, `perf:`, `build:`, `ci:`.
- One logical change per commit.
- **Never commit secrets, keys, tokens, or build artifacts.**
- `pubspec.lock` is committed (reproducible builds) - do not gitignore it.
- Generated code (`.g.dart`, `.freezed.dart`, drift output) is committed.

## 3. Pull requests

- Target `main`; CI gates (when wired) run format/analyze/test.
- Review against `REVIEW_CHECKLIST.md` and `DEFINITION_OF_DONE.md`.
- Update `.ai/` docs (status/debt/decisions) in the same PR when the change
  affects them.

## 4. Release & hotfix

- Semantic versioning in `pubspec.yaml`; tag releases; update changelog.
- **Hotfix workflow:** branch `hotfix/<slug>` from the tagged release -> fix ->
  test -> merge to `main` -> cherry-pick to release branch -> tag patch bump.

## 5. Current conventions (mobile)

- Working branch at P004: `mobile_project_foundation`.
- Sprint-0 prompts are delivered as `P00x` milestones; each milestone is
  verified (format/analyze/test/build) before commit.

## 6. Reference

- `mobile/docs/12-GitWorkflow.md`
- `.github/` (CI/CD definitions)
