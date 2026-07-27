# Security Assessment

> **Module:** TrainingProgramManagement
> **Date:** 2026-07-26
> **Assessment Type:** Static Code Analysis
> **Severity Scale:** Critical > High > Medium > Low

---

## Findings Summary

| Severity | Count | Details |
|----------|-------|---------|
| Critical | 2 | Hardcoded secrets, missing input validation |
| High | 3 | CORS misconfiguration risk, no rate limiting, search exposes internal IDs |
| Medium | 4 | Fragile error handling leaks internal details, no request logging, health check unauthenticated, SaveToken=true |
| Low | 3 | No anti-forgery (acceptable for JWT-only), HSTS manual instead of `UseHSTS`, ClockSkew |

---

## Critical Findings

### SEC-001: JWT Signing Key Hardcoded

| Field | Value |
|-------|-------|
| **Severity** | Critical |
| **CVSS** | 9.8 |
| **Location** | `appsettings.json:17` |
| **Risk** | Full token forgery if repository is compromised. An attacker with the signing key can mint valid JWTs for any user and any role. |

**Remediation:**

- Move the JWT signing key to Azure Key Vault, AWS Secrets Manager, or environment variables
- Rotate the key immediately upon externalization
- Never commit secrets to source control — add `appsettings.json` to `.gitignore` if it contains secrets

---

### SEC-002: Database Credentials Hardcoded

| Field | Value |
|-------|-------|
| **Severity** | Critical |
| **CVSS** | 9.8 |
| **Location** | `appsettings.json:10` |
| **Risk** | Direct database access if repository is compromised. Full data exfiltration or destruction possible. |

**Remediation:**

- Use secrets manager or environment variables for connection strings
- Enable Transparent Data Encryption (TDE) on the database
- Restrict database access to application subnet only

---

## High Findings

### SEC-003: CORS Fallback to Localhost

| Field | Value |
|-------|-------|
| **Severity** | High |
| **Location** | `Program.cs:140-158` |
| **Risk** | If CORS configuration is empty or misconfigured, localhost origins are allowed with credentials. This enables CSRF-like attacks from local development tools. |

**Remediation:**

- Fail fast in production if no CORS origins are explicitly configured
- Never include localhost in production CORS policy
- Validate CORS configuration at startup

---

### SEC-004: No Rate Limiting on Training Endpoints

| Field | Value |
|-------|-------|
| **Severity** | High |
| **Location** | All training controllers |
| **Risk** | API abuse and denial-of-service. Unbounded request rates can exhaust database connections, memory, and CPU. |

**Remediation:**

- Add `[EnableRateLimiting]` attributes to all training controllers
- Configure rate limit policies per endpoint tier (search: 30/min, writes: 60/min, reads: 120/min)
- Implement global rate limiting as a fallback

---

### SEC-005: Search Endpoint Exposes Internal Academy IDs

| Field | Value |
|-------|-------|
| **Severity** | High |
| **Location** | `TrainingProgramsController.cs:87-116` |
| **Risk** | Unauthenticated enumeration of internal academy IDs via the public search endpoint. Attackers can map the academy structure. |

**Remediation:**

- Remove `academyId` from public search responses, or
- Add rate limiting specifically to the search endpoint
- Consider using opaque identifiers instead of sequential/database IDs

---

## Medium Findings

### SEC-006: HandleFailure Leaks Internal IDs via Error Messages

| Field | Value |
|-------|-------|
| **Severity** | Medium |
| **Location** | All 8 training controllers |
| **Risk** | The `error.Contains("not found")` pattern passes raw GUIDs in 404 responses, exposing internal database identifiers. |

**Remediation:**

- Return generic "Resource not found" messages in production
- Log the full error (with IDs) server-side for debugging
- Use a shared error mapping utility

---

### SEC-007: No Structured Request Logging

| Field | Value |
|-------|-------|
| **Severity** | Medium |
| **Location** | Application pipeline |
| **Risk** | No middleware for request/response logging. Cannot audit who accessed what, when, or detect abuse patterns. |

**Remediation:**

- Add structured request logging middleware (method, path, status, duration, user)
- Integrate with centralized logging (Serilog + Seq / ELK / CloudWatch)
- Redact sensitive fields (passwords, tokens) from logs

---

### SEC-008: Health Check Endpoint Unauthenticated

| Field | Value |
|-------|-------|
| **Severity** | Medium |
| **Location** | `/health` endpoint |
| **Risk** | Exposes system status (database connectivity, dependency health) without authentication. |

**Remediation:**

- Keep `/health` unauthenticated for load balancer probes
- Add a separate `/health/detailed` endpoint with authentication for operational visibility
- Restrict `/health` to internal network only if possible

---

### SEC-009: SaveToken=true Wastes Memory

| Field | Value |
|-------|-------|
| **Severity** | Medium |
| **Location** | Authentication configuration |
| **Risk** | JWT stored in `HttpContext` unnecessarily, consuming memory per request. Minor but unnecessary attack surface. |

**Remediation:**

- Set `SaveToken = false` if the token is not needed in `HttpContext`
- Access the token from the `Authorization` header directly

---

## Low Findings

### SEC-010: No Anti-Forgery Token Configuration

| Field | Value |
|-------|-------|
| **Severity** | Low |
| **Risk** | No anti-forgery tokens configured. **Acceptable** for a JWT-only API where CSRF is not applicable. |

No action required for JWT-only APIs. Re-evaluate if cookie-based auth is added.

---

### SEC-011: HSTS Configured Manually

| Field | Value |
|-------|-------|
| **Severity** | Low |
| **Location** | Security headers middleware |
| **Risk** | HSTS header set manually instead of using `app.UseHSTS()`. Minor deviation from ASP.NET Core best practices. |

**Remediation:** Replace manual HSTS header with `app.UseHSTS()`.

---

### SEC-012: JWT ClockSkew

| Field | Value |
|-------|-------|
| **Severity** | Low |
| **Risk** | Default `ClockSkew` of 5 minutes allows expired tokens to remain valid. |

**Remediation:** Set `ClockSkew = TimeSpan.Zero` or a small value (30s) and ensure server clocks are synchronized via NTP.

---

## Positive Security Controls

| Control | Status | Notes |
|---------|--------|-------|
| JWT Authentication | ✅ Implemented | Proper signing with HMAC-SHA256 |
| Role-Based Authorization | ✅ Implemented | All training endpoints require roles |
| Swagger Gating | ✅ Implemented | Only accessible in Development environment |
| HTTPS Enforcement | ✅ Configured | `UseHttpsRedirection` present |
| Security Headers Middleware | ✅ Present | Custom middleware adds security headers |
| FluentValidation | ✅ Implemented | All command objects validated |
| Soft Delete Pattern | ✅ Implemented | `IsDeleted` flag on entities |
| Global Exception Handler | ✅ Present | Returns generic errors in production |

---

## Remediation Priority

| Finding | Severity | Effort | Priority |
|---------|----------|--------|----------|
| SEC-001: JWT Key Hardcoded | Critical | 2h | P0 |
| SEC-002: DB Credentials Hardcoded | Critical | 2h | P0 |
| SEC-003: CORS Fallback | High | 1h | P0 |
| SEC-004: No Rate Limiting | High | 4h | P1 |
| SEC-005: Search Exposes IDs | High | 2h | P1 |
| SEC-006: Error Message Leaks | Medium | 3h | P2 |
| SEC-007: No Request Logging | Medium | 4h | P2 |
| SEC-008: Health Check Auth | Medium | 1h | P2 |
| SEC-009: SaveToken | Medium | 30m | P2 |
| SEC-010: Anti-Forgery | Low | 0 | N/A |
| SEC-011: HSTS | Low | 30m | P3 |
| SEC-012: ClockSkew | Low | 15m | P3 |
