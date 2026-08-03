# DEFINITION OF DONE

Status: **Adopted** - Owner: Chief Software Architect

A feature (or change) is **done only when every applicable item below
passes**. Nothing ships as "done" with outstanding items unless the exception
is recorded in `TECH_DEBT.md` and approved.

## 1. Build passes
- [ ] `flutter build web` succeeds (offline target); platform builds are CI-gated.

## 2. Analyzer passes
- [ ] `dart format --set-exit-if-changed lib test integration_test` clean.
- [ ] `flutter analyze` reports **zero issues**.

## 3. Tests pass
- [ ] `flutter test` green; new tests added for changed/new behaviour.
- [ ] Error/empty/loading states tested; mocks in `test/mocks/`.

## 4. Documentation updated
- [ ] `.ai/` updated: `SPRINT_STATUS.md`, and `DECISIONS.md`/`TECH_DEBT.md`/
      `CHANGELOG.md` as applicable.
- [ ] Feature and sprint docs (`mobile/docs/`, `docs/mobile/`) reflect the
      change.

## 5. API verified
- [ ] Integration code matches `docs/api/openapi.yaml` exactly - no invented
      endpoints, fields, or response shapes.
- [ ] Error mapping (`NetworkErrorKind` -> feature `Failure`) is correct.

## 6. Responsive UI
- [ ] Works across phone/tablet/wide breakpoints; no fixed-pixel layouts.

## 7. Accessibility
- [ ] Labels, contrast, touch targets, and text scaling satisfied
      (`UI_GUIDELINES.md` Section 3).

## 8. Localization
- [ ] All user-facing strings in ARB (en, hi, mr); `flutter gen-l10n` run;
      no hardcoded text.

## 9. Code reviewed
- [ ] Self-reviewed against `REVIEW_CHECKLIST.md` (and human/PR review where
      applicable).

## 10. Performance checked
- [ ] Lazy lists/pagination; no network/DB in `build`; rebuild churn avoided
      (`PERFORMANCE.md`).

## 11. Security reviewed
- [ ] No secrets committed; sensitive data in `SecureStorage`; no sensitive
      logging; input validated (`SECURITY.md`).

---

### Sign-off

All items checked -> the change is done. Any unchecked item must be listed in
the prompt's final report with a reason and an owner.
