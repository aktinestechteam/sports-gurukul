# Deployment Checklist

> **Module:** TrainingProgramManagement
> **Date:** 2026-07-26
> **Target Environment:** Production

---

## Pre-Deployment Checklist

### P0 — MUST Complete (Blockers)

These items **must** be completed before any production deployment. Skipping these will result in data loss or security breaches.

- [ ] Add `SaveChangesAsync` to all 26 command handlers
- [ ] Externalize JWT signing key to secrets manager
- [ ] Externalize database connection string to secrets manager
- [ ] Add `HasQueryFilter(e => !e.IsDeleted)` to 8 entity configurations
- [ ] Run EF Core migration after query filter changes

---

### P1 — SHOULD Complete (High Priority)

These items significantly impact production reliability and should be completed before go-live.

- [ ] Add `RowVersion` to Attendance, AssessmentResult, TrainingGoal, TrainingMilestone, TrainingMaterial, SessionSchedule entities
- [ ] Run EF Core migration after RowVersion changes
- [ ] Fix N+1 query patterns in UpdateTrainingProgress, IssueCertificate, GetTrainingStatistics handlers
- [ ] Fix in-memory filtering in SearchTrainingPrograms, GetAthleteEnrollments handlers
- [ ] Fix null-forgiving operator issues in Session/Batch handlers
- [ ] Add rate limiting `[EnableRateLimiting]` to all training controllers
- [ ] Add validation for inline request types in controllers
- [ ] Fix duplicate `DbSet<TrainingCertificate>` in `ApplicationDbContext`

---

### P2 — NICE to Complete (Medium Priority)

These items improve code quality and maintainability. Can be addressed post-launch.

- [ ] Extract `HandleFailure` to shared base controller or use typed errors
- [ ] Extract `MapToDto` to shared static methods
- [ ] Add pagination to `GetBatchesByProgram`, `GetSessionsByBatch`, `GetSessionAttendance`, `GetAssessmentResults`
- [ ] Add structured request logging middleware
- [ ] Add CORS production configuration validation (fail fast)
- [ ] Fix `PublishAssessmentResultsCommandHandler` no-op
- [ ] Fix `MarkAttendanceCommandHandler` null Athlete
- [ ] Fix late check-in logic in `CheckInAthleteCommandHandler`

---

### Infrastructure

Infrastructure items required for production readiness.

- [ ] Configure production connection string
- [ ] Configure CORS allowed origins for production domain
- [ ] Disable Swagger in production (verify `ASPNETCORE_ENVIRONMENT` is not `Development`)
- [ ] Configure rate limiting defaults
- [ ] Set up monitoring / APM (Application Insights / Datadog / New Relic)
- [ ] Configure log aggregation
- [ ] Set up health check monitoring
- [ ] Configure HTTPS certificates
- [ ] Set up CI/CD pipeline with test execution
- [ ] Configure auto-scaling rules

---

### Testing

Testing items to verify before deployment.

- [ ] Run all unit tests (531 tests, all should pass)
- [ ] Run integration tests against staging database
- [ ] Perform load testing (target: 100 concurrent users)
- [ ] Test authentication flows end-to-end
- [ ] Test authorization (401/403 for all endpoints)
- [ ] Test error scenarios (invalid input, not found, conflicts)

---

## Post-Deployment Verification

Verify the following items immediately after deployment.

- [ ] Verify health check endpoint responds
- [ ] Verify authentication works with production JWT
- [ ] Verify CORS headers for production domain
- [ ] Verify rate limiting is enforced
- [ ] Monitor error rates for first 24 hours
- [ ] Verify database migrations applied correctly
- [ ] Verify Swagger is not accessible

---

## Sign-Off

| Role | Name | Date | Signed |
|------|------|------|--------|
| Backend Team Lead | | | [ ] |
| Security Team | | | [ ] |
| DevOps / Infrastructure | | | [ ] |
| Product Owner | | | [ ] |
