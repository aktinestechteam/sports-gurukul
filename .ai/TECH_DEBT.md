# TECH_DEBT

Status: **Living** - Owner: Chief Software Architect

Every compromise, shortcut, and known limitation is logged here so it is
either paid down or deliberately kept. Do not hide debt - the knowledge base
is only trustworthy if it is honest.

## Entry template

```markdown
## TD-NNN - <Short title>

- **Added:** YYYY-MM-DD - **Prompt:** P0xx
- **Impact:** Low | Medium | High
- **Owner:** <feature/module>
- **Description:** <what was deferred and why>
- **Pay-down plan:** <when/how it will be resolved>
- **Status:** Open | In progress | Closed (resolved in <prompt>)
```

## Open items

### TD-001 - Placeholder sample model not yet removed
- **Added:** P003
- **Impact:** Medium
- **Owner:** models
- **Description:** `sample_model.dart` (+ `.freezed.dart`, `.g.dart`,
  `sample_model_test.dart`) exists only to validate freezed/json codegen.
- **Pay-down plan:** Remove when the first real domain model lands (P005+).
- **Status:** Open

### TD-002 - AuthInterceptor is a placeholder
- **Added:** P003
- **Impact:** High
- **Owner:** network/auth
- **Description:** The auth interceptor exists in the chain but does not
  inject or refresh JWTs yet.
- **Pay-down plan:** Implement with the auth feature (P005): read from
  `SecureStorage`, centralized 401 refresh/relogin.
- **Status:** Open

### TD-003 - Base URL hard-coded as empty in ApiClient
- **Added:** P003
- **Impact:** Medium
- **Owner:** config/network
- **Description:** `ApiClient.create()` defaults `baseUrl` to `''`; env-driven
  base URL resolution is not yet wired.
- **Pay-down plan:** Resolve base URL from `app/config/environment.dart` +
  `app_config.dart` with the first real integration.
- **Status:** Open

### TD-004 - Riverpod codegen deferred (analyzer conflict)
- **Added:** P003
- **Impact:** Low (workaround: hand-written providers)
- **Owner:** state
- **Description:** See `DECISIONS.md` ADR-004.
- **Pay-down plan:** Revisit when `freezed`/`riverpod_generator` analyzer
  requirements align; adopt codegen + riverpod_lint then.
- **Status:** Open

### TD-005 - No CI pipeline yet
- **Added:** P002/P003
- **Impact:** Medium
- **Owner:** devops
- **Description:** Format/analyze/test are run locally only; `.github/`
  defines backend workflows but the mobile gate is not wired.
- **Pay-down plan:** Add a mobile workflow (format, analyze, test, coverage)
  gating PRs.
- **Status:** Open

### TD-006 - No AppRadius / AppTypography tokens yet
- **Added:** P003
- **Impact:** Low
- **Owner:** design system
- **Description:** `AppColors` and `AppSpacing` exist; radius and typography
  are still theme-derived rather than tokenized.
- **Pay-down plan:** Design-system sprint adds the remaining token holders.
- **Status:** Open

### TD-007 - Certificate pinning not implemented
- **Added:** P003 (future requirement)
- **Impact:** Low (pre-auth)
- **Owner:** security
- **Description:** See `SECURITY.md` Section 4; pinning is deliberately future.
- **Pay-down plan:** Introduce before production release, coordinated with the
  backend certificate rotation policy.
- **Status:** Open

### TD-008 - Goldens are platform-specific
- **Added:** P003
- **Impact:** Low
- **Owner:** testing
- **Description:** `matchesGoldenFile` goldens render with the Ahem test font
  and differ across platforms.
- **Pay-down plan:** Regenerate per platform in CI; or adopt a font-loading
  harness if cross-platform goldens are required.
- **Status:** Open

## Closed items

### TD-C001 - `url_strategy` package avoided (discontinued)
- **Resolved in:** P003 - using built-in `usePathUrlStrategy()` when needed.
- **Status:** Closed

### TD-C002 - `golden_toolkit` avoided (discontinued)
- **Resolved in:** P003 - built-in `matchesGoldenFile`.
- **Status:** Closed
