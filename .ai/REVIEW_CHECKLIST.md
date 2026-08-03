# REVIEW_CHECKLIST

Status: **Adopted** - Owner: Chief Software Architect

Every change is self-reviewed against this checklist before it is reported
done. Reviewers use the same list.

## 1. Architecture
- [ ] Clean Architecture respected; no `presentation` -> API/DB access.
- [ ] Domain has no Flutter/HTTP/JSON dependency.
- [ ] DTOs stayed inside `infrastructure/`; mapped to entities at the boundary.
- [ ] No cross-feature imports; shared logic promoted to `core/` or `shared/`.
- [ ] Dependency injection via Riverpod; no global singletons/service locators.

## 2. Security
- [ ] No secrets/keys/tokens committed; no sensitive data logged.
- [ ] Tokens live in `SecureStorage`; injected via `AuthInterceptor`.
- [ ] HTTPS only; input validated; PII minimized.
- [ ] No debug backdoors; no plaintext secrets in prefs/Drift.

## 3. Performance
- [ ] Lists use lazy builders + pagination; no unbounded loads.
- [ ] `const` constructors used; no network/DB work in `build`.
- [ ] Animations GPU-friendly; heavy subtrees isolated (`RepaintBoundary`).
- [ ] No avoidable rebuild churn (selectors, stable keys).

## 4. Accessibility
- [ ] Semantic labels on icon-only controls; tooltips where needed.
- [ ] Contrast OK in light and dark themes; touch targets >= 48x48.
- [ ] No color-only state signalling; text scaling respected.

## 5. Localization
- [ ] No hardcoded user-facing strings; all from ARB (en, hi, mr);
      `flutter gen-l10n` run.
- [ ] Placeholders/pluralization used instead of manual concatenation.

## 6. Testing
- [ ] New behaviour covered (unit/widget/integration as appropriate).
- [ ] Error and empty states tested; mocks via mocktail; goldens regenerated.
- [ ] `flutter analyze` clean; `flutter test` green.

## 7. Documentation
- [ ] `.ai/` updated when relevant (SPRINT_STATUS, DECISIONS, TECH_DEBT,
      CHANGELOG).
- [ ] Feature/sprint docs updated; README pointers intact.

## 8. Maintainability
- [ ] Follows naming conventions (`mobile/docs/11-NamingConvention.md`).
- [ ] No duplicated logic; reuse existing facades/providers/tokens.
- [ ] Small, readable widgets; typed failures, no raw exceptions to UI.

## Result

- [ ] All applicable items pass -> mark complete.
- [ ] Any item fails -> fix before reporting done (or record a deliberate
      exception in `TECH_DEBT.md`).
