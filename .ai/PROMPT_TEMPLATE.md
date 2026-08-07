# PROMPT_TEMPLATE

Status: **Adopted** - Owner: Chief Software Architect

## Mandatory preamble for every AI prompt

Every prompt given to OpenCode, Cursor, Claude Code, GitHub Copilot or any
AI assistant **must** begin with the following directive. The AI must read
the entire `.ai/` directory before making any code change.

```text
## GOVERNANCE

Before any code change:
1. Read EVERY file in `.ai/` (PROJECT_CONTEXT, PROJECT_RULES, ARCHITECTURE,
   FOLDER_STRUCTURE, CODING_STANDARDS, FLUTTER_STANDARDS, STATE_MANAGEMENT,
   NETWORKING, DATABASE, DESIGN_SYSTEM, UI_GUIDELINES, BACKEND_INTEGRATION,
   API_GUIDELINES, SECURITY, PERFORMANCE, TESTING, GIT_WORKFLOW,
   SPRINT_STATUS, TECH_DEBT, DECISIONS, CHANGELOG, PROMPT_TEMPLATE,
   REVIEW_CHECKLIST, DEFINITION_OF_DONE).
2. Read `mobile/docs/` and `docs/mobile/` as needed for depth.
3. For any API work, read `docs/api/openapi.yaml` (the single source of truth)
   and follow `BACKEND_INTEGRATION.md` + `API_GUIDELINES.md`.
4. State the plan and the verification commands you will run BEFORE writing code.
```

## Standard prompt body

```markdown
## TITLE
P<number> - <one-line summary>

## ROLE
You are the <role> for this task.

## OBJECTIVE
<clear, single objective>

## CONTEXT
<what exists, what is out of scope, what must not change>

## SCOPE / DELIVERABLES
- <list of concrete files/documents/features to create or modify>

## CONSTRAINTS
- <contracts: backend endpoints, architecture, allowed packages>
- Never: <redesign backend | invent APIs | violate Clean Architecture |
          add non-approved dependencies | hardcode strings/colors>
- Only add dependencies via the process in `mobile/docs/10-DependencyGuide.md`.

## VERIFICATION (run all, report output)
- [ ] `dart format --set-exit-if-changed lib test integration_test`
- [ ] `flutter analyze`  -> zero issues
- [ ] `flutter test`     -> green (new tests where behaviour changed)
- [ ] `flutter build web` -> success
- [ ] Docs updated: SPRINT_STATUS / DECISIONS / TECH_DEBT / CHANGELOG as needed

## OUTPUT
Before implementation: plan + rationale.
After implementation: summary, verification results, and any follow-ups.
```

## Rules for the AI assistant

1. Follow `PROJECT_RULES.md` - all nine rules are binding.
2. Do not skip code review: self-review against `REVIEW_CHECKLIST.md` and
   satisfy `DEFINITION_OF_DONE.md` before reporting completion.
3. If something contradicts the governance docs, ask - never improvise.
4. Update the temporal docs (SPRINT_STATUS, DECISIONS, TECH_DEBT, CHANGELOG)
   when the change affects them.
