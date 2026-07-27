# Risk Register

> **Project:** SportsGurukul Training Module  
> **Last Updated:** 2026-07-26  
> **Total Risks:** 12  
> **High:** 6 | **Medium:** 4 | **Low:** 2

---

## Risk Scoring Matrix

| | Impact: Low | Impact: Medium | Impact: High | Impact: Critical |
|---|---|---|---|---|
| **Probability: Certain** | Medium | High | Critical | Critical |
| **Probability: High** | Low | Medium | High | Critical |
| **Probability: Medium** | Low | Medium | Medium | High |
| **Probability: Low** | Low | Low | Medium | Medium |

---

## High Risks

| Risk ID | Description | Probability | Impact | Score | Mitigation Strategy | Owner | Status |
|---------|-------------|-------------|--------|-------|---------------------|-------|--------|
| R-001 | **Data loss from missing `SaveChangesAsync`.** 26 of 32 command handlers never call `SaveChangesAsync`, meaning all write operations silently discard data. Every user action that creates, updates, or deletes a record is lost. | Certain | Critical | Critical | Fix all 26 handlers before any deployment. Add integration tests that verify data persistence after each command. | Backend Team | Open |
| R-002 | **Database breach from hardcoded secrets.** Connection strings with credentials are stored in source code (e.g., `appsettings.json` or handler files). Secrets are exposed in version control history, CI logs, and deployment artifacts. | Medium | Critical | High | Externalize all secrets to Azure Key Vault or environment variables. Rotate credentials immediately. Add pre-commit hooks to block secret commits. | DevOps | Open |
| R-003 | **OOM / service crash from N+1 queries and in-memory filtering.** N+1 patterns cause O(N) database round-trips per query. In-memory filtering loads entire tables. Under production load, both will exhaust memory and database connections. | High | High | High | Replace N+1 patterns with `Include()` / projection queries. Replace in-memory filtering with `IQueryable`-based SQL `WHERE` clauses. Add query performance tests. | Backend Team | Open |
| R-004 | **Data corruption from missing concurrency tokens.** 6 entities lack `RowVersion` columns. Concurrent updates silently overwrite each other without detection, causing silent data loss. | Medium | High | Medium | Add `RowVersion` column and `IsRowVersion()` configuration to all 6 entities. Handle `DbUpdateConcurrencyException` in handlers. | Backend Team | Open |
| R-005 | **Soft-deleted data leak into production queries.** 8 entity configurations are missing `HasQueryFilter(e => !e.IsDeleted)`. Soft-deleted records appear in all default queries, exposing data that should be hidden. | Certain | Medium | High | Add `HasQueryFilter` to all 8 entity configurations. Add integration tests that verify soft-deleted records are excluded. | Backend Team | Open |
| R-006 | **500 errors from null reference in re-fetched entities.** 12+ handlers use null-forgiving operators on entities re-fetched from the database. If the entity was deleted between the initial fetch and re-fetch, a `NullReferenceException` produces an unhandled 500 error. | Medium | Medium | Medium | Add null checks after re-fetch or map DTOs from the initially-fetched in-memory entity instead of re-querying. Return 404 when entity is not found. | Backend Team | Open |

---

## Medium Risks

| Risk ID | Description | Probability | Impact | Score | Mitigation Strategy | Owner | Status |
|---------|-------------|-------------|--------|-------|---------------------|-------|--------|
| R-007 | **Silent HTTP status code changes from string-matching error handling.** The `HandleFailure` method in 8 controllers maps error messages to HTTP status codes using string comparison. Any message change silently alters the returned status code, breaking API consumers. | High | Low | Medium | Implement typed error codes or an enum-based error system. Return structured error responses with machine-readable codes instead of relying on message text. | Backend Team | Open |
| R-008 | **API abuse from missing rate limiting.** No `[EnableRateLimiting]` attributes on any training controller. Endpoints are vulnerable to traffic spikes, scraping, and denial-of-service patterns. | Medium | Medium | Medium | Add `[EnableRateLimiting]` to all training controllers. Configure appropriate throttle limits in `Program.cs` based on endpoint sensitivity. | Backend Team | Open |
| R-009 | **Unbounded responses from missing pagination.** 4 list endpoints return all matching records in a single response. As data grows, responses will exceed memory limits and saturate network bandwidth. | Medium | Medium | Medium | Add `pageNumber` and `pageSize` query parameters to all list endpoints. Default to a reasonable page size (e.g., 25). Return total count in response metadata. | Backend Team | Open |
| R-010 | **Invalid data from missing controller-level validation.** Inline request records have no `FluentValidation` validators. Invalid or malicious input reaches handlers and database without sanitization. | Medium | Low | Low | Create `AbstractValidator<T>` classes for all inline request types. Register validators in DI. Return 400 with validation errors before handler execution. | Backend Team | Open |

---

## Low Risks

| Risk ID | Description | Probability | Impact | Score | Mitigation Strategy | Owner | Status |
|---------|-------------|-------------|--------|-------|---------------------|-------|--------|
| R-011 | **Duplicate records from race conditions.** Check-then-act patterns in program creation, enrollment, and transfer are not protected by database constraints or application-level locks. Concurrent requests can produce duplicate records. | Low | Medium | Low | Add unique database constraints on natural keys (e.g., program name + batch). Alternatively, use distributed locking (`IDistributedLock`) for critical operations. | Backend Team | Open |
| R-012 | **Feature non-functional: `PublishAssessmentResults`.** The handler returns a success response without performing any work. Callers (including the controller) believe assessment results were published when they were not. | Certain | Low | Medium | Implement the actual publishing logic in `PublishAssessmentResultsCommandHandler`. Add integration tests that verify side effects. | Backend Team | Open |

---

## Risk Summary

| Category | Count | Critical | High | Medium | Low |
|----------|-------|----------|------|--------|-----|
| Data Integrity | 4 | 1 | 2 | 1 | 0 |
| Security | 1 | 0 | 1 | 0 | 0 |
| Performance | 1 | 0 | 1 | 0 | 0 |
| Reliability | 2 | 0 | 1 | 0 | 1 |
| API Quality | 3 | 0 | 0 | 2 | 1 |
| Feature Completeness | 1 | 0 | 0 | 0 | 1 |
| **Total** | **12** | **1** | **5** | **3** | **2** |

---

## Remediation Timeline

| Phase | Risks | Target | Dependencies |
|-------|-------|--------|--------------|
| **Phase 1 — Ship Blockers** | R-001, R-002 | Before any deployment | None |
| **Phase 2 — Data Safety** | R-004, R-005, R-006 | Week 1 | Phase 1 |
| **Phase 3 — Performance** | R-003, R-009 | Week 2 | Phase 1 |
| **Phase 4 — Hardening** | R-007, R-008, R-010 | Week 3 | None |
| **Phase 5 — Polish** | R-011, R-012 | Week 4 | None |

---

*This register should be reviewed weekly. Risks that have been mitigated should be marked as **Closed** with the date and verification method.*
