# Deployment Checklist — Communication Platform

> **Module:** CommunicationPlatform
> **Date:** 2026-07-30
> **Target Environment:** Production

---

## Pre-Deployment Checklist

### P0 — MUST Complete (Blockers)

These items **must** be completed before any production deployment. Skipping these will result in data loss or security breaches.

- [ ] Add `SaveChangesAsync` to all notification services (NotificationService, TemplateService, PreferenceService, CampaignService)
- [ ] Externalize JWT signing key to secrets manager
- [ ] Externalize database connection string to secrets manager
- [ ] Externalize SMTP credentials to secrets manager
- [ ] Disable Swagger in production (verify `ASPNETCORE_ENVIRONMENT` is not `Development`)

---

### P1 — SHOULD Complete (High Priority)

These items significantly impact production reliability and should be completed before go-live.

- [ ] Wire `AddCommunicationPlatform()` into `Program.cs` and add project reference
- [ ] Add `ICampaignRepository` with EF Core persistence
- [ ] Create/verify EF Core migration for notification domain entities
- [ ] Add pagination limits (server-enforced max page size)
- [ ] Add structured request logging middleware with correlation IDs
- [ ] Integrate OpenTelemetry / APM (Application Insights / Datadog)
- [ ] Add per-user rate limiting for notification endpoints
- [ ] Extract `HandleFailure` to shared base controller or use typed errors

---

### P2 — NICE to Complete (Medium Priority)

These items improve code quality and maintainability. Can be addressed post-launch.

- [ ] Add unit tests for individual notification handlers/services
- [ ] Add load/performance tests for notification endpoints
- [ ] Add pagination to GetStatistics endpoints
- [ ] Add CORS production configuration validation (fail fast)
- [ ] Configure log aggregation (Elasticsearch / Splunk / Loki)

---

### Infrastructure

Infrastructure items required for production readiness.

- [ ] Configure production database connection string
- [ ] Configure CORS allowed origins for production domain
- [ ] Configure rate limiting defaults (adjust 100 req/min if needed)
- [ ] Set up monitoring / APM (Application Insights / Datadog / New Relic)
- [ ] Configure log aggregation
- [ ] Set up health check monitoring (alert on `/health` failure)
- [ ] Configure HTTPS certificates
- [ ] Set up CI/CD pipeline with test execution (69 integration tests)
- [ ] Configure auto-scaling rules for notification processing

---

### Testing

Testing items to verify before deployment.

- [ ] Run all unit tests
- [ ] Run all 69 integration tests
- [ ] Perform load testing (target: 100 concurrent notification sends/min)
- [ ] Test authentication flows end-to-end (Admin, Athlete, roles)
- [ ] Test authorization (401/403 for all endpoints)
- [ ] Test error scenarios (invalid input, not found, conflicts)
- [ ] Test notification lifecycle (create → queue → send → read → cancel)
- [ ] Test campaign operations (create → schedule → pause → resume → cancel)
- [ ] Test preference operations (subscribe → mute → unmute → unsubscribe)
- [ ] Verify webhook callbacks work end-to-end

---

## Post-Deployment Verification

Verify the following items immediately after deployment.

- [ ] Verify health check endpoint responds (`GET /health`)
- [ ] Verify authentication works with production JWT
- [ ] Verify CORS headers for production domain
- [ ] Verify rate limiting is enforced (429 responses on abuse)
- [ ] Monitor error rates for first 24 hours
- [ ] Verify database migrations applied correctly
- [ ] Verify Swagger is not accessible
- [ ] Verify notification creation and retrieval works
- [ ] Verify template creation, publishing, and versioning works
- [ ] Verify preference management (mute/unmute) works
- [ ] Verify delivery status queries work
- [ ] Verify queue reprocess works
- [ ] Verify campaign lifecycle works

---

## Sign-Off

| Role | Name | Date | Signed |
|---|---|---|---|
| Backend Team Lead | | | [ ] |
| Security Team | | | [ ] |
| DevOps / Infrastructure | | | [ ] |
| Product Owner | | | [ ] |
