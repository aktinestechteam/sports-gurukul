# Deployment Checklist — Academy Module

**Date:** 2026-07-25
**Module:** Academy Management
**Environment:** Production
**Status:** ⚠️ CONDITIONAL — Requires P0/P1 fixes

---

## Pre-Deployment Checklist

### Code Quality

| # | Item | Status | Owner | Due Date |
|---|------|--------|-------|----------|
| 1 | Build succeeds (0 errors, 0 warnings) | ✅ | - | - |
| 2 | Unit tests pass (789 tests) | ✅ | - | - |
| 3 | Integration tests pass (166 tests) | ❌ Docker not running | TBD | ASAP |
| 4 | Code review completed | ✅ | - | - |
| 5 | No merge conflicts | ✅ | - | - |

### Critical Fixes (Must Complete)

| # | Item | Status | Owner | Due Date |
|---|------|--------|-------|----------|
| 6 | Fix in-memory pagination (`GetPagedAcademiesQueryHandler`) | ❌ OPEN | TBD | P0 |
| 7 | Standardize `HandleFailure` across all controllers | ❌ OPEN | TBD | P0 |
| 8 | Route `AcademySearchRepository` writes through UnitOfWork | ❌ OPEN | TBD | P1 |
| 9 | Add `RowVersion` to 10 Academy entities | ❌ OPEN | TBD | P1 |
| 10 | Add database indexes for search columns | ❌ OPEN | TBD | P1 |
| 11 | Escape LIKE special characters in search queries | ❌ OPEN | TBD | P1 |
| 12 | Add rate limiting to search endpoints | ❌ OPEN | TBD | P1 |

### Database

| # | Item | Status | Owner | Due Date |
|---|------|--------|-------|----------|
| 13 | EF Core migrations up to date | ⚠️ Verify | TBD | Pre-deploy |
| 14 | Migration scripts tested | ⚠️ Verify | TBD | Pre-deploy |
| 15 | Rollback strategy documented | ❌ OPEN | TBD | Pre-deploy |
| 16 | Database backup verified | ⚠️ Verify | TBD | Pre-deploy |
| 17 | Indexes created and tested | ❌ OPEN | TBD | Pre-deploy |

### Security

| # | Item | Status | Owner | Due Date |
|---|------|--------|-------|----------|
| 18 | JWT key rotation configured | ⚠️ Verify | TBD | Pre-deploy |
| 19 | HTTPS enforced | ✅ | - | - |
| 20 | CORS configured correctly | ⚠️ Verify | TBD | Pre-deploy |
| 21 | Rate limiting implemented | ❌ OPEN | TBD | Pre-deploy |
| 22 | Input validation complete | ✅ | - | - |
| 23 | SQL injection prevention verified | ✅ | - | - |
| 24 | Swagger disabled in production | ❌ OPEN | TBD | Pre-deploy |

### Configuration

| # | Item | Status | Owner | Due Date |
|---|------|--------|-------|----------|
| 25 | Connection strings in secrets manager | ⚠️ Verify | TBD | Pre-deploy |
| 26 | Environment variables configured | ⚠️ Verify | TBD | Pre-deploy |
| 27 | Logging configured (Serilog/Application Insights) | ⚠️ Verify | TBD | Pre-deploy |
| 28 | Health check endpoints added | ❌ OPEN | TBD | Pre-deploy |

### Monitoring & Observability

| # | Item | Status | Owner | Due Date |
|---|------|--------|-------|----------|
| 29 | Application Insights configured | ⚠️ Verify | TBD | Pre-deploy |
| 30 | Error alerting configured | ⚠️ Verify | TBD | Pre-deploy |
| 31 | Performance monitoring enabled | ⚠️ Verify | TBD | Pre-deploy |
| 32 | Distributed tracing configured | ❌ OPEN | TBD | Post-deploy |

### Infrastructure

| # | Item | Status | Owner | Due Date |
|---|------|--------|-------|----------|
| 33 | Container image built and tested | ⚠️ Verify | TBD | Pre-deploy |
| 34 | Kubernetes manifests updated | ⚠️ Verify | TBD | Pre-deploy |
| 35 | Resource limits configured | ⚠️ Verify | TBD | Pre-deploy |
| 36 | Horizontal pod autoscaler configured | ⚠️ Verify | TBD | Pre-deploy |

---

## Deployment Steps

### 1. Pre-Deployment (T-1 hour)

- [ ] Run full test suite (unit + integration)
- [ ] Verify database migrations are up to date
- [ ] Check application configuration
- [ ] Verify secrets are accessible
- [ ] Run security scan (Snyk/OWASP)
- [ ] Notify stakeholders of deployment

### 2. Database Migration (T-30 min)

- [ ] Take database backup
- [ ] Run EF Core migrations:
  ```bash
  dotnet ef database update --project backend/src/SportsGurukul.Infrastructure
  ```
- [ ] Verify migration success
- [ ] Create rollback script (if needed)

### 3. Application Deployment (T-0)

- [ ] Deploy to staging environment
- [ ] Run smoke tests
- [ ] Verify health checks pass
- [ ] Deploy to production (blue-green or rolling)
- [ ] Monitor for errors

### 4. Post-Deployment (T+15 min)

- [ ] Verify all endpoints responding
- [ ] Check error rates
- [ ] Verify logging working
- [ ] Run integration tests against production
- [ ] Monitor performance metrics

### 5. Rollback Plan (If Needed)

- [ ] Identify failure (error rate spike, performance degradation)
- [ ] Rollback application to previous version
- [ ] Rollback database migration (if needed)
- [ ] Notify stakeholders
- [ ] Document incident

---

## Environment Configuration

### Production Settings

| Setting | Value | Source |
|---------|-------|--------|
| ASPNETCORE_ENVIRONMENT | Production | Environment Variable |
| ConnectionStrings__DefaultConnection | *** | Secrets Manager |
| Jwt__Key | *** | Secrets Manager |
| Jwt__Issuer | *** | Environment Variable |
| Jwt__Audience | *** | Environment Variable |
| Logging__LogLevel__Default | Warning | Configuration |

### Resource Limits

| Resource | Request | Limit |
|----------|---------|-------|
| CPU | 250m | 1000m |
| Memory | 256Mi | 512Mi |
| Replicas | 2 | 5 |

### Health Checks

| Endpoint | Path | Purpose |
|----------|------|---------|
| Liveness | `/health` | Application is running |
| Readiness | `/ready` | Application is ready to serve traffic |

---

## Monitoring & Alerting

### Key Metrics

| Metric | Threshold | Action |
|--------|-----------|--------|
| Error Rate | >1% | Alert |
| Response Time (p95) | >500ms | Alert |
| CPU Usage | >80% | Alert |
| Memory Usage | >80% | Alert |
| Database Connections | >80% | Alert |

### Alert Channels

| Alert | Channel | Recipients |
|-------|---------|------------|
| Critical | PagerDuty | On-call engineer |
| Warning | Slack | #deployments |
| Info | Email | Team lead |

---

## Rollback Procedure

### Application Rollback

```bash
# Rollback to previous version
kubectl rollout undo deployment/sportsgurukul-api -n production

# Verify rollback
kubectl rollout status deployment/sportsgurukul-api -n production
```

### Database Rollback

```bash
# Rollback last migration
dotnet ef database update PreviousMigration --project backend/src/SportsGurukul.Infrastructure

# Verify rollback
dotnet ef migrations list --project backend/src/SportsGurukul.Infrastructure
```

---

## Post-Deployment Tasks

### Immediate (T+1 hour)

- [ ] Verify application health
- [ ] Check error rates
- [ ] Verify logging working
- [ ] Monitor performance

### Short-Term (T+24 hours)

- [ ] Review error logs
- [ ] Check performance metrics
- [ ] Gather user feedback
- [ ] Document any issues

### Long-Term (T+1 week)

- [ ] Conduct deployment retrospective
- [ ] Update documentation
- [ ] Plan next iteration
- [ ] Address remaining technical debt

---

## Sign-Off

| Role | Name | Date | Signature |
|------|------|------|-----------|
| Tech Lead | | | |
| QA Lead | | | |
| DevOps Lead | | | |
| Product Owner | | | |

---

*Checklist maintained by OpenCode Deployment Automation*
*Review cycle: Before each deployment*
