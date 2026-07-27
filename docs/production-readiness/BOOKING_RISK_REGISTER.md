# Risk Register — Booking & Scheduling Module

> **Module:** BookingSchedulingManagement
> **Date:** 2026-07-27
> **Total Risks:** 14
> **Critical:** 2 | **High:** 4 | **Medium:** 5 | **Low:** 3

---

## Risk Scoring Matrix

| | Impact: Low | Impact: Medium | Impact: High | Impact: Critical |
|---|---|---|---|---|
| **Probability: Certain** | Medium | High | Critical | Critical |
| **Probability: High** | Low | Medium | High | Critical |
| **Probability: Medium** | Low | Medium | Medium | High |
| **Probability: Low** | Low | Low | Medium | Medium |

---

## Critical Risks

| Risk ID | Description | Probability | Impact | Score | Mitigation Strategy | Owner | Status |
|---------|-------------|-------------|--------|-------|---------------------|-------|--------|
| R-BKG-001 | **Hardcoded secrets in source control.** JWT signing key and database credentials are in `appsettings.json`. If the repository is compromised, an attacker can forge JWTs for any user/role and access the database directly. Secrets are also exposed in CI/CD logs and Docker images. | Medium | Critical | High | Externalize to environment variables or secrets manager (Azure Key Vault / AWS Secrets Manager). Rotate credentials immediately. Add pre-commit hooks to block secret commits. | DevOps / Backend | Open |
| R-BKG-002 | **CORS localhost fallback in production.** When `Cors:AllowedOrigins` is not configured, the CORS policy allows `localhost:3000` and `localhost:5001` with credentials. If deployed without CORS configuration, any local tool can make authenticated cross-origin requests. | Medium | Critical | High | Add startup validation that fails if no CORS origins are configured in non-Development environments. Never include localhost in production CORS. | Backend / DevOps | Open |

---

## High Risks

| Risk ID | Description | Probability | Impact | Score | Mitigation Strategy | Owner | Status |
|---------|-------------|-------------|--------|-------|---------------------|-------|--------|
| R-BKG-003 | **No rate limiting on booking controllers.** All 8 booking controllers lack `[EnableRateLimiting]` attributes despite rate limit policies being configured in `Program.cs`. The booking API is vulnerable to abuse, scraping, and denial-of-service. | High | High | High | Add `[EnableRateLimiting("default")]` to all booking controllers. Configure per-endpoint limits: search=30/min, writes=60/min, reads=120/min. | Backend | Open |
| R-BKG-004 | **Cancellation and reschedule audit records not persisted.** `CancelBookingCommandHandler` and `RescheduleBookingCommandHandler` create domain entities but never add them to repositories. No audit trail exists for these critical operations. | Certain | High | Critical | Add `IBookingCancellationRepository` and `IBookingRescheduleRepository` injections. Call `AddAsync` + `SaveChangesAsync` for audit entities. | Backend | Open |
| R-BKG-005 | **Approval records not persisted.** `BookingApprovalService.ProcessApprovalAsync` modifies the last approval entity in memory but the changes are only saved if the caller calls `SaveChangesAsync`. `CreateApprovalRequestAsync` returns an entity that is never added to any repository. | Medium | High | Medium | Wire approval repository into `BookingApprovalService`. Persist approval records via `IUnitOfWork.SaveChangesAsync`. | Backend | Open |
| R-BKG-006 | **No user context in booking commands.** `BookingCreatorId` is never populated from JWT claims. Commands don't know who created, cancelled, or modified a booking. This makes it impossible to enforce ownership-based authorization or produce meaningful audit logs. | High | Medium | Medium | Inject `ICurrentUser` into command handlers. Populate `BookingCreatorId` from `ICurrentUser.UserId` in `CreateBookingCommandHandler`. | Backend | Open |

---

## Medium Risks

| Risk ID | Description | Probability | Impact | Score | Mitigation Strategy | Owner | Status |
|---------|-------------|-------------|--------|-------|---------------------|-------|--------|
| R-BKG-007 | **Fragile string-matching error handling.** All 8 booking controllers use `error.Contains("not found")` to map to 404. If any handler changes its error message, the HTTP status silently changes, breaking API consumers. | High | Low | Medium | Implement typed error codes (enum or error code string) in `Result<T>.Error` instead of free-text messages. Map error codes to HTTP status in a shared utility. | Backend | Open |
| R-BKG-008 | **In-memory statistics aggregation.** `GetBookingStatisticsQueryHandler` loads ALL bookings for a date range into memory and computes aggregates via LINQ. For academies with thousands of bookings per month, this causes high memory usage and slow responses. | Medium | Medium | Medium | Replace with SQL-level aggregation queries (COUNT, GROUP BY). Add Redis caching for frequently accessed statistics. | Backend | Open |
| R-BKG-009 | **Integration tests failing.** Both `Booking.IntegrationTests` and `SportsGurukul.IntegrationTests` fail during database migration due to seed data ordering issues (FK violations and null RowVersion). CI/CD cannot validate integration behavior. | Certain | Medium | High | Fix seed data ordering in migrations. Ensure referenced entities (Academies, Sports) are seeded before dependent entities (Bookings). | Backend / QA | Open |
| R-BKG-010 | **No correlation ID support.** No middleware generates or propagates correlation IDs. In a distributed system, tracing a request across multiple handlers and services is impossible. | Medium | Medium | Medium | Add correlation ID middleware that generates a GUID per request, stores it in `HttpContext.Items`, and adds it to response headers and log scopes. | Backend | Open |
| R-BKG-011 | **No request logging middleware.** No middleware logs incoming requests, response status codes, or duration. Production troubleshooting relies on scattered handler-level logs. | Medium | Medium | Medium | Add request logging middleware: method, path, status code, duration, user ID. Integrate with Serilog structured logging. | Backend | Open |

---

## Low Risks

| Risk ID | Description | Probability | Impact | Score | Mitigation Strategy | Owner | Status |
|---------|-------------|-------------|--------|-------|---------------------|-------|--------|
| R-BKG-012 | **`HandleFailure` and `GetUserId` duplicated across controllers.** 5 controllers have identical helper methods. A logic change requires updating all 5 copies. | Low | Low | Low | Extract to a shared `BookingControllerBase` class or static utility. | Backend | Open |
| R-BKG-013 | **Booking number generation has no max retry.** `SchedulingEngine.GenerateBookingNumberAsync` loops until a unique number is found with no max iteration limit. Theoretically, if the random component always collides, the loop runs forever. | Low | Medium | Low | Add a max retry count (e.g., 10). After exhausting retries, throw an exception instead of looping indefinitely. | Backend | Open |
| R-BKG-014 | **Inconsistent API route naming.** `/api/v1/booking-statistics` uses singular-hyphenated while `/api/v1/bookings` uses plural. Minor UX inconsistency for API consumers. | Low | Low | Low | Standardize to `/api/v1/booking-statistics` → `/api/v1/booking-stats` or similar. | Backend | Open |

---

## Risk Summary

| Category | Count | Critical | High | Medium | Low |
|----------|-------|----------|------|--------|-----|
| Security | 2 | 1 | 1 | 0 | 0 |
| Data Integrity | 3 | 0 | 2 | 1 | 0 |
| API Quality | 2 | 0 | 0 | 1 | 1 |
| Performance | 1 | 0 | 0 | 1 | 0 |
| Testing | 1 | 0 | 0 | 1 | 0 |
| Observability | 2 | 0 | 0 | 2 | 0 |
| Reliability | 2 | 1 | 1 | 0 | 0 |
| Code Quality | 1 | 0 | 0 | 0 | 1 |
| **Total** | **14** | **2** | **4** | **5** | **3** |

---

## Remediation Timeline

| Phase | Risks | Target | Dependencies |
|-------|-------|--------|--------------|
| **Phase 1 — Security** | R-BKG-001, R-BKG-002 | Before any deployment | None |
| **Phase 2 — Data Integrity** | R-BKG-004, R-BKG-005, R-BKG-006 | Before any deployment | None |
| **Phase 3 — API Protection** | R-BKG-003, R-BKG-007 | Week 1 | None |
| **Phase 4 — Observability** | R-BKG-010, R-BKG-011 | Week 1 | None |
| **Phase 5 — Performance** | R-BKG-008 | Week 2 | None |
| **Phase 6 — Testing** | R-BKG-009 | Week 1 | Phase 2 |
| **Phase 7 — Polish** | R-BKG-012, R-BKG-013, R-BKG-014 | Week 3 | None |

---

*This register should be reviewed weekly. Risks that have been mitigated should be marked as **Closed** with the date and verification method.*
