# Security Assessment — Academy Module

**Date:** 2026-07-25
**Module:** Academy Management
**Assessor:** OpenCode Security Review
**Overall Score:** 70/100 — PASS with Concerns

---

## Executive Summary

The Academy module implements security best practices for authentication and authorization. However, several concerns exist around input validation, rate limiting, and error handling consistency.

**Key Strengths:**
- JWT Bearer authentication properly configured
- Role-based access control (RBAC) on all endpoints
- `[Authorize]` at class level on all controllers
- System Admin role required for sensitive operations (Verify Academy)

**Critical Concerns:**
1. **LIKE injection** in search queries (potential data exfiltration)
2. **No rate limiting** on search/discovery endpoints
3. **HandleFailure string-matching** could expose internal error messages
4. **No input sanitization** for LIKE wildcards

---

## Authentication

### JWT Configuration

**Status:** ✅ PASS

**Implementation:**
- JWT Bearer authentication configured in `Program.cs`
- Token validation for issuer, audience, and lifetime
- Secure key storage (environment variables)

**Findings:**
- ✅ Token expiration enforced
- ✅ Signature validation enabled
- ✅ HTTPS required for token transmission
- ⚠️ No token refresh mechanism documented
- ⚠️ No key rotation strategy documented

**Recommendations:**
1. Implement JWT key rotation (quarterly)
2. Add token refresh endpoint
3. Document key management procedures

---

## Authorization

### Role-Based Access Control (RBAC)

**Status:** ✅ PASS

**Implementation:**
- `[Authorize(Roles = "...")]` on all endpoints
- Roles: System Admin, Academy Admin, Coach, Athlete
- Class-level `[Authorize]` on all controllers

**Endpoint Matrix:**

| Endpoint | System Admin | Academy Admin | Coach | Athlete |
|----------|--------------|---------------|-------|---------|
| Create Academy | ✅ | ❌ | ❌ | ❌ |
| Verify Academy | ✅ | ❌ | ❌ | ❌ |
| Update Academy | ✅ | ✅ | ❌ | ❌ |
| Delete Academy | ✅ | ✅ | ❌ | ❌ |
| Get Academy | ✅ | ✅ | ✅ | ✅ |
| Assign Coach | ✅ | ✅ | ❌ | ❌ |
| Register Athlete | ✅ | ✅ | ❌ | ❌ |
| Search Academies | ✅ | ✅ | ✅ | ✅ |

**Findings:**
- ✅ All write operations require elevated privileges
- ✅ Read operations accessible to authenticated users
- ✅ System Admin can verify academies
- ⚠️ No ownership validation (Academy Admin can update any academy)

**Recommendations:**
1. Add ownership validation (Academy Admin can only update their academy)
2. Add resource-based authorization
3. Document RBAC model

---

## Input Validation

### FluentValidation

**Status:** ✅ PASS

**Implementation:**
- 29 validators for all AcademyManagement commands
- Validation pipeline behavior in MediatR
- RFC7807 ProblemDetails for validation errors

**Findings:**
- ✅ All commands have validators
- ✅ Required fields enforced
- ✅ String length limits enforced
- ✅ Email format validated
- ✅ GUID format validated

**Sample Validator:**
```csharp
public class CreateAcademyValidator : AbstractValidator<CreateAcademyCommand>
{
    public CreateAcademyValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Academy name is required.")
            .MaximumLength(200).WithMessage("Academy name must not exceed 200 characters.");
    }
}
```

**Recommendations:**
1. Add XSS protection for rich text fields
2. Add SQL injection prevention (already using parameterized queries)
3. Add input sanitization for LIKE patterns

---

## SQL Injection Prevention

### Parameterized Queries

**Status:** ✅ PASS

**Implementation:**
- EF Core parameterized queries
- No raw SQL usage
- `EF.Functions.Like()` with parameterized inputs

**Findings:**
- ✅ All queries use parameterized execution
- ✅ No string concatenation for SQL
- ✅ EF Core prevents SQL injection

**⚠️ Exception — LIKE Injection:**
```csharp
// Current (VULNERABLE):
var term = searchTerm.ToLowerInvariant();
query = query.Where(a =>
    EF.Functions.Like(a.Name.ToLower(), $"%{term}%"));

// Attack: Search term "%password%" matches all records
```

**Recommendation:**
Escape LIKE special characters:
```csharp
private static string EscapeLike(string input) =>
    input.Replace("%", "\\%").Replace("_", "\\_");

var term = EscapeLike(searchTerm.ToLowerInvariant());
```

---

## Error Handling

### Global Exception Handler

**Status:** ⚠️ CONDITIONAL

**Implementation:**
- Global exception handler in `Program.cs`
- RFC7807 ProblemDetails for errors
- Logging of exceptions

**Findings:**
- ✅ Global exception handler catches unhandled exceptions
- ✅ RFC7807 ProblemDetails format
- ✅ Exception details logged
- ⚠️ `HandleFailure` string-matching could expose internal messages
- ⚠️ Inconsistent error handling across controllers

**Risks:**
- Error messages could contain sensitive information
- String-matching could silently change HTTP status codes
- Different controllers handle errors differently

**Recommendation:**
1. Replace string-matching with typed error codes
2. Sanitize error messages before returning to client
3. Standardize error handling across all controllers

---

## Rate Limiting

### Current State

**Status:** ❌ FAIL

**Implementation:** None

**Findings:**
- ❌ No rate limiting on any endpoints
- ❌ No protection against brute force attacks
- ❌ No protection against scraping
- ❌ No protection against DoS attacks

**Risks:**
- Search endpoints could be abused for data exfiltration
- Brute force attacks on authentication endpoints
- Service degradation under load

**Recommendations:**
1. Add rate limiting to all endpoints (30 req/min per user)
2. Add stricter limits on sensitive endpoints (5 req/min)
3. Add CAPTCHA for anonymous users
4. Add IP-based rate limiting for public endpoints

---

## Data Protection

### Sensitive Data Handling

**Status:** ✅ PASS

**Implementation:**
- Soft delete for data retention
- Audit fields for data tracking
- HTTPS for data transmission

**Findings:**
- ✅ No sensitive data in logs (PII)
- ✅ Passwords hashed (using Identity)
- ✅ Tokens not logged
- ⚠️ No data encryption at rest
- ⚠️ No data masking for sensitive fields

**Recommendations:**
1. Implement field-level encryption for sensitive data (GST Number, Registration Number)
2. Add data masking for PII in logs
3. Document data retention policies

---

## API Security

### Swagger/OpenAPI

**Status:** ✅ PASS

**Implementation:**
- Swagger UI enabled in development
- JWT Bearer token support in Swagger
- API versioning

**Findings:**
- ✅ Swagger UI for API exploration
- ✅ JWT token support
- ✅ API versioning (v1)
- ⚠️ Swagger exposed in production (should be disabled)

**Recommendations:**
1. Disable Swagger in production
2. Add API key authentication for external consumers
3. Document API security requirements

---

## Security Checklist

| Category | Item | Status | Priority |
|----------|------|--------|----------|
| Authentication | JWT configured | ✅ | - |
| Authentication | Token expiration | ✅ | - |
| Authentication | Key rotation | ❌ | P2 |
| Authorization | RBAC implemented | ✅ | - |
| Authorization | Ownership validation | ❌ | P2 |
| Input Validation | FluentValidation | ✅ | - |
| Input Validation | LIKE injection prevention | ❌ | P2 |
| SQL Injection | Parameterized queries | ✅ | - |
| Error Handling | Global exception handler | ✅ | - |
| Error Handling | Typed error codes | ❌ | P1 |
| Rate Limiting | Endpoint rate limiting | ❌ | P1 |
| Data Protection | Soft delete | ✅ | - |
| Data Protection | Audit fields | ✅ | - |
| Data Protection | Encryption at rest | ❌ | P3 |
| API Security | Swagger disabled in prod | ❌ | P2 |

---

## Vulnerability Summary

| Severity | Count | Status |
|----------|-------|--------|
| Critical | 0 | - |
| High | 2 | OPEN |
| Medium | 4 | OPEN |
| Low | 3 | OPEN |

### High Vulnerabilities

1. **LIKE Injection (H-001)**
   - **File:** `AcademySearchRepository.cs`
   - **Risk:** Data exfiltration, performance degradation
   - **Fix:** Escape LIKE special characters
   - **Effort:** 2-3 hours

2. **No Rate Limiting (H-002)**
   - **Files:** All controllers
   - **Risk:** DoS, scraping, brute force
   - **Fix:** Add rate limiting middleware
   - **Effort:** 3-4 hours

### Medium Vulnerabilities

1. **HandleFailure String-Matching (M-001)**
   - **Files:** 5 controllers
   - **Risk:** Inconsistent HTTP status codes
   - **Fix:** Use typed error codes
   - **Effort:** 6-8 hours

2. **No Ownership Validation (M-002)**
   - **Files:** AcademyController
   - **Risk:** Unauthorized modifications
   - **Fix:** Add resource-based authorization
   - **Effort:** 4-6 hours

3. **Swagger in Production (M-003)**
   - **File:** `Program.cs`
   - **Risk:** API exposure
   - **Fix:** Disable Swagger in production
   - **Effort:** 1 hour

4. **No Data Masking (M-004)**
   - **Files:** Logging
   - **Risk:** PII exposure in logs
   - **Fix:** Add data masking
   - **Effort:** 2-3 hours

### Low Vulnerabilities

1. **No Key Rotation (L-001)**
2. **No Encryption at Rest (L-002)**
3. **No CAPTCHA (L-003)**

---

## Recommendations

### Immediate (Pre-Deployment)
1. **P1:** Add rate limiting to search endpoints
2. **P1:** Replace HandleFailure string-matching with typed error codes
3. **P1:** Escape LIKE special characters in search queries

### Short-Term (Post-Deploy)
1. **P2:** Add ownership validation for Academy Admin
2. **P2:** Disable Swagger in production
3. **P2:** Add data masking for PII in logs
4. **P2:** Implement JWT key rotation

### Long-Term (Future Sprints)
1. **P3:** Add field-level encryption for sensitive data
2. **P3:** Add CAPTCHA for anonymous users
3. **P3:** Implement API key authentication

---

*Security Assessment generated by OpenCode Security Review*
*Next review: After P1 fixes are implemented*
