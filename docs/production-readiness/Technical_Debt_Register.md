# Technical Debt Register

> **Project:** SportsGurukul Training Module  
> **Last Updated:** 2026-07-26  
> **Total Items:** 22  
> **Critical:** 6 | **Medium:** 8 | **Minor:** 8

---

## Summary

| Severity | Count | Total Effort |
|----------|-------|--------------|
| Critical | 6 | 7-9.5 days |
| Medium | 8 | 5.5 days |
| Minor | 8 | 4.75 days |
| **Total** | **22** | **17.25-19.75 days** |

---

## Critical Debt Items

| ID | Description | Effort | Impact | Affected Files |
|----|-------------|--------|--------|----------------|
| TD-001 | Missing `SaveChangesAsync` in 26 of 32 command handlers. Data is never persisted to the database. | 2-3 days | Zero data persistence | All handlers in `Session/`, `Batch/`, `Enrollment/`, `Attendance/`, `Assessment/`, `Progress/` subdirectories |
| TD-002 | N+1 query patterns. Each entity in a result set triggers a separate database round-trip for related data. | 1-2 days | O(N) DB round-trips per query | `UpdateTrainingProgressCommandHandler.cs:31-47`, `IssueCertificateCommandHandler.cs:44-54`, `GetTrainingStatisticsQueryHandler.cs:50-78` |
| TD-003 | In-memory filtering. Full table loads from database then LINQ-filtered in application memory instead of using SQL `WHERE`. | 2-3 days | Loads entire tables into memory | `SearchTrainingProgramsQueryHandler.cs:28`, `GetAthleteEnrollmentsQueryHandler.cs:26` |
| TD-004 | 8 entity configurations missing `HasQueryFilter` for soft delete. Soft-deleted records are returned by default queries. | 0.5 day | Soft-deleted records leak into all queries | `AttendanceConfiguration.cs`, `TrainingProgressConfiguration.cs`, `TrainingCertificateConfiguration.cs`, `TrainingGoalConfiguration.cs`, `TrainingMilestoneConfiguration.cs`, `TrainingMaterialConfiguration.cs`, `SessionScheduleConfiguration.cs`, `AssessmentResultConfiguration.cs` |
| TD-005 | Missing `RowVersion` concurrency token on 6 entities. No optimistic concurrency protection — write conflicts are silently overwritten. | 1 day | Silent write conflicts / data overwrites | All 6 entity configuration files + corresponding entity class files: Attendance, AssessmentResult, TrainingGoal, TrainingMilestone, TrainingMaterial, SessionSchedule |
| TD-006 | Null-forgiving operator (`!`) applied to re-fetched entities in 12+ handlers. If the entity was deleted between fetch and re-fetch, a `NullReferenceException` occurs at runtime. | 1 day | `NullReferenceException` → unhandled 500 errors | All Session and Batch command handlers |

---

## Medium Debt Items

| ID | Description | Effort | Impact | Affected Files |
|----|-------------|--------|--------|----------------|
| TD-007 | Duplicated `MapToDto` methods copied across 12 Session/Batch handlers. Any DTO change requires updating all 12 copies. | 1 day | Maintenance burden, inconsistent mapping between handlers | All Session/Batch command handlers |
| TD-008 | Duplicated `HandleFailure` method in all 8 training controllers. Uses fragile string matching to map error messages to HTTP status codes. | 0.5 day | Fragile string matching for HTTP status codes; silent incorrect status on message changes | All training controllers |
| TD-009 | Inline request records without `FluentValidation` validators. No input validation at the controller level. | 1 day | No input validation at controller level; invalid data reaches handlers | `TrainingBatchesController.cs`, `TrainingSessionsController.cs`, `AttendanceController.cs`, `AssessmentsController.cs`, `CertificatesController.cs`, `ProgressController.cs` |
| TD-010 | Inconsistent error messages across handlers. Some expose internal GUIDs to API consumers. | 0.5 day | API UX inconsistency; potential information leakage | Multiple handlers |
| TD-011 | Race conditions on check-then-act patterns. No database-level constraints or application-level locks protect concurrent creation. | 2-3 days | Duplicate programs, over-capacity enrollments under concurrent requests | `CreateTrainingProgramCommandHandler.cs`, `EnrollAthleteCommandHandler.cs`, `TransferEnrollmentCommandHandler.cs` |
| TD-012 | Late check-in logic bug in `CheckInAthleteCommandHandler`. Threshold calculation produces incorrect attendance classification. | 0.25 day | Incorrect attendance classification (late marked as on-time or vice versa) | `CheckInAthleteCommandHandler.cs:47` |
| TD-013 | `PublishAssessmentResultsCommandHandler` is a no-op. Handler returns success without performing any work. | 0.5 day | Feature doesn't work; callers believe results were published | `PublishAssessmentResultsCommandHandler.cs:45` |
| TD-014 | `MarkAttendanceCommandHandler` always sets Athlete to null in DTO. Re-fetched entity's navigation property is never populated. | 0.25 day | Empty athlete name in returned DTO | `MarkAttendanceCommandHandler.cs:73` |

---

## Minor Debt Items

| ID | Description | Effort | Impact | Affected Files |
|----|-------------|--------|--------|----------------|
| TD-015 | Inconsistent constructor styles. Some handlers use traditional constructors; others use C# 12 primary constructors. | 0.5 day | Code style inconsistency across the codebase | Multiple handler files |
| TD-016 | Duplicate `DbSet<TrainingCertificate>` declaration in `ApplicationDbContext`. Two entry points to the same table can cause EF Core tracking issues. | 0.25 day | Two entry points to the same table | `ApplicationDbContext.cs:93-94` |
| TD-017 | Infinite loop risk in code generation without max retries. If the generated value never meets uniqueness constraints, the loop runs forever. | 0.25 day | Potential application hang / thread starvation | `CreateTrainingSessionHandler.cs:85-89`, `CreateTrainingBatchHandler.cs:54-58` |
| TD-018 | `CreatedBy` / `UpdatedBy` fields ignored in all entity configurations. No values are ever populated. | 1 day | No audit trail for who created or modified records | All `*Configuration.cs` files |
| TD-019 | Duplicate `SearchProgramsAlias` route registered. Two routes map to the same controller action. | 0.25 day | Confusing API surface; ambiguous route resolution | `TrainingProgramsController.cs:131-160` |
| TD-020 | No pagination on 4 list endpoints. Unbounded result sets are returned in a single response. | 1 day | Unbounded response sizes; potential memory and network issues at scale | `TrainingBatchesController.cs`, `TrainingSessionsController.cs`, `AttendanceController.cs`, `AssessmentsController.cs` |
| TD-021 | No request logging middleware. No structured logging of incoming requests, responses, or timing. | 1 day | Poor production diagnostics; difficulty troubleshooting issues | `Program.cs` |
| TD-022 | Missing rate limiting attributes on training controllers. No throttling on any training module endpoints. | 0.5 day | No API protection against abuse or traffic spikes | All training controllers |

---

## Priority Order for Remediation

| Phase | Items | Rationale |
|-------|-------|-----------|
| **Phase 1 — Blocking** | TD-001 | Zero data persistence is a ship-blocker |
| **Phase 2 — Data Integrity** | TD-004, TD-005, TD-016 | Prevent data leaks and corruption |
| **Phase 3 — Performance** | TD-002, TD-003, TD-020 | Prevent OOM and unbounded responses under load |
| **Phase 4 — Reliability** | TD-006, TD-011, TD-012, TD-013, TD-014, TD-017 | Prevent runtime crashes and incorrect behavior |
| **Phase 5 — Code Quality** | TD-007, TD-008, TD-009, TD-010, TD-015, TD-018, TD-019 | Reduce maintenance burden |
| **Phase 6 — Production Ops** | TD-021, TD-022 | Enable production monitoring and protection |

---

*This register should be reviewed and updated weekly during remediation sprints.*
