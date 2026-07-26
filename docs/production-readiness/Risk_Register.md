# Risk Register — Academy Module

**Date:** 2026-07-25
**Module:** Academy Management
**Total Risks:** 12
**Risk Score:** 6.2/10 (Medium-High)

---

## Risk Matrix

| Risk ID | Category | Risk | Probability | Impact | Score | Status |
|---------|----------|------|-------------|--------|-------|--------|
| R-001 | Performance | In-memory pagination causes OOM | High | Critical | 9 | OPEN |
| R-002 | Reliability | HandleFailure inconsistency returns wrong HTTP status | High | High | 8 | OPEN |
| R-003 | Security | LIKE injection in search queries | Medium | High | 6 | OPEN |
| R-004 | Scalability | No rate limiting on search endpoints | High | Medium | 6 | OPEN |
| R-005 | Reliability | UnitOfWork bypass causes partial writes | Medium | High | 6 | OPEN |
| R-006 | Reliability | Missing RowVersion causes data loss | Medium | High | 6 | OPEN |
| R-007 | Quality | Integration tests unvalidated | High | Medium | 6 | OPEN |
| R-008 | Performance | No database indexes for search | Medium | Medium | 4 | OPEN |
| R-009 | Reliability | No retry logic for transient failures | Medium | Medium | 4 | OPEN |
| R-010 | Maintainability | 35+ parameter search method | Medium | Low | 3 | OPEN |
| R-011 | Observability | No distributed tracing | Low | Medium | 2 | OPEN |
| R-012 | Observability | No health check endpoints | Low | Low | 1 | OPEN |

**Risk Score = Probability (1-3) × Impact (1-3)**

---

## Detailed Risk Analysis

### R-001: In-Memory Pagination Causes OOM

**Category:** Performance
**Probability:** High (3/3) — Will occur with 10K+ academies
**Impact:** Critical (3/3) — Application crash, service outage
**Risk Score:** 9/9

**Description:**
`GetPagedAcademiesQueryHandler` loads ALL academies into memory via `GetAllAsync()`. With 10K+ academies, this will cause:
- Memory spike (potentially 100MB+ per request)
- Garbage collection pressure
- Out-of-memory exception under load

**Triggers:**
- Academy count exceeds 10K
- Multiple concurrent requests
- Memory-constrained environment (container with 512MB limit)

**Mitigation:**
- P0 fix: Replace with DB-level pagination
- Add memory monitoring and alerts
- Set container memory limits

**Residual Risk:** Low (1/3) after fix

---

### R-002: HandleFailure Inconsistency

**Category:** Reliability
**Probability:** High (3/3) — Error messages change frequently
**Impact:** High (3/3) — Wrong HTTP status codes, client confusion
**Risk Score:** 8/9

**Description:**
`HandleFailure` uses string-matching on error messages. When error messages change (e.g., during refactoring), HTTP status codes silently change:
- "Academy not found" → 404 ✅
- "Academy not found by ID" → 400 ❌ (should be 404)
- "Already registered" → 409 in AthletesController, 400 in others

**Triggers:**
- Error message refactoring
- New error messages added
- Localization (different languages)

**Mitigation:**
- P0 fix: Use typed error codes
- Add integration tests for HTTP status codes
- Document error message contracts

**Residual Risk:** Low (1/3) after fix

---

### R-003: LIKE Injection in Search Queries

**Category:** Security
**Probability:** Medium (2/3) — Requires malicious user input
**Impact:** High (3/3) — Data exfiltration, performance degradation
**Risk Score:** 6/9

**Description:**
User input is passed directly to `EF.Functions.Like()` without escaping `%` and `_` characters. An attacker could:
- Use `%` to match all records (data exfiltration)
- Use `_` to match single characters (pattern matching)
- Craft complex patterns to extract sensitive data

**Example Attack:**
```
Search term: %password%
Result: Returns all academies (bypasses search filter)
```

**Mitigation:**
- P2 fix: Escape LIKE special characters
- Add input validation (max length, allowed characters)
- Rate limit search endpoints

**Residual Risk:** Low (1/3) after fix

---

### R-004: No Rate Limiting on Search Endpoints

**Category:** Scalability
**Probability:** High (3/3) — Search endpoints are public-facing
**Impact:** Medium (2/3) — Performance degradation, potential outage
**Risk Score:** 6/9

**Description:**
Search and discovery endpoints have no rate limiting. Attackers or bots could:
- Scrape all academy data
- Cause database overload
- Degrade service for legitimate users

**Triggers:**
- Bot traffic
- Scraper scripts
- DoS attacks

**Mitigation:**
- P2 fix: Add rate limiting (30 req/min per user)
- Add CAPTCHA for anonymous users
- Monitor request patterns

**Residual Risk:** Low (1/3) after fix

---

### R-005: UnitOfWork Bypass

**Category:** Reliability
**Probability:** Medium (2/3) — Occurs on every search write
**Impact:** High (3/3) — Partial writes, data inconsistency
**Risk Score:** 6/9

**Description:**
`AcademySearchRepository` bypasses `IUnitOfWork.SaveChangesAsync()`, causing:
- No transaction isolation
- No audit logging
- No event publishing
- Partial writes on failure

**Triggers:**
- Database failure during save
- Concurrent modifications
- Audit requirement

**Mitigation:**
- P1 fix: Route through IUnitOfWork
- Add transaction logging
- Monitor write consistency

**Residual Risk:** Low (1/3) after fix

---

### R-006: Missing RowVersion

**Category:** Reliability
**Probability:** Medium (2/3) — Concurrent updates likely
**Impact:** High (3/3) — Data loss (last-write-wins)
**Risk Score:** 6/9

**Description:**
10 Academy entities lack `RowVersion` for optimistic concurrency. Concurrent updates can cause:
- Silent data overwrites
- Lost updates
- Data inconsistency

**Triggers:**
- Multiple users editing same entity
- Bulk import operations
- Race conditions

**Mitigation:**
- P1 fix: Add RowVersion to all entities
- Add conflict detection in handlers
- Add retry logic for conflicts

**Residual Risk:** Low (1/3) after fix

---

### R-007: Integration Tests Unvalidated

**Category:** Quality
**Probability:** High (3/3) — Docker not running
**Impact:** Medium (2/3) — Unknown bugs in production
**Risk Score:** 6/9

**Description:**
166 integration test cases are written but never executed. Unknown bugs could exist in:
- Database migrations
- Repository implementations
- Controller logic
- Authentication/authorization

**Triggers:**
- Docker unavailable
- Test environment issues
- CI/CD pipeline gaps

**Mitigation:**
- P0 fix: Start Docker, run tests
- Add integration tests to CI/CD
- Monitor test coverage

**Residual Risk:** Low (1/3) after fix

---

### R-008: Missing Database Indexes

**Category:** Performance
**Probability:** Medium (2/3) — Search queries will slow down
**Impact:** Medium (2/3) — Slow response times
**Risk Score:** 4/9

**Description:**
No composite indexes for common search patterns. Queries will degrade as data grows:
- Full table scans on Name, Email
- Slow JOIN operations
- Poor query plan optimization

**Triggers:**
- Academy count exceeds 1K
- Complex search queries
- High concurrent load

**Mitigation:**
- P1 fix: Add indexes for search columns
- Monitor query performance
- Use EXPLAIN to validate plans

**Residual Risk:** Low (1/3) after fix

---

### R-009: No Retry Logic

**Category:** Reliability
**Probability:** Medium (2/3) — Transient failures occur
**Impact:** Medium (2/3) — Service unavailability
**Risk Score:** 4/9

**Description:**
No Polly retry policies for transient database failures:
- Connection timeouts
- Deadlocks
- Network glitches

**Triggers:**
- Database overload
- Network issues
- Connection pool exhaustion

**Mitigation:**
- P1 fix: Add retry-on-failure (3 retries, exponential backoff)
- Add circuit breaker for external services
- Monitor failure rates

**Residual Risk:** Low (1/3) after fix

---

### R-010: 35+ Parameter Search Method

**Category:** Maintainability
**Probability:** Medium (2/3) — Changes require signature updates
**Impact:** Low (1/3) — Developer productivity
**Risk Score:** 3/9

**Description:**
`SearchAcademiesAsync` has 35+ parameters, making it:
- Difficult to maintain
- Error-prone (wrong parameter order)
- Hard to extend (adding new filters)

**Mitigation:**
- P2 fix: Refactor to query object pattern
- Add unit tests for parameter validation
- Document API contract

**Residual Risk:** Low (1/3) after fix

---

### R-011: No Distributed Tracing

**Category:** Observability
**Probability:** Low (1/3) — Infrastructure issue
**Impact:** Medium (2/3) — Difficult debugging
**Risk Score:** 2/9

**Description:**
No OpenTelemetry or distributed tracing. Difficult to:
- Trace requests across services
- Identify bottlenecks
- Debug production issues

**Mitigation:**
- P3 fix: Add OpenTelemetry
- Add correlation IDs
- Monitor request flow

**Residual Risk:** Low (1/3) after fix

---

### R-012: No Health Check Endpoints

**Category:** Observability
**Probability:** Low (1/3) — Infrastructure issue
**Impact:** Low (1/3) — Delayed detection
**Risk Score:** 1/9

**Description:**
No health check endpoints for:
- Database connectivity
- External service availability
- Application readiness

**Mitigation:**
- P3 fix: Add `/health` and `/ready` endpoints
- Add Kubernetes liveness/readiness probes
- Monitor health status

**Residual Risk:** Low (1/3) after fix

---

## Risk Trends

| Metric | Value |
|--------|-------|
| Total Risks | 12 |
| Critical (9) | 1 |
| High (6-8) | 1 |
| Medium (4-5) | 4 |
| Low (1-3) | 6 |
| Open Risks | 12 |
| Mitigated Risks | 0 |

---

*Register maintained by OpenCode Risk Management*
*Review cycle: Weekly*
