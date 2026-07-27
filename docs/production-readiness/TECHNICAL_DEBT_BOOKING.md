# Technical Debt Register — Booking & Scheduling Module

> **Module:** BookingSchedulingManagement
> **Date:** 2026-07-27
> **Total Items:** 21
> **Critical:** 4 | **Medium:** 9 | **Low:** 8

---

## Summary

| Severity | Count | Total Effort |
|----------|-------|--------------|
| Critical | 4 | 1.5–2 days |
| Medium | 9 | 4–6 days |
| Low | 8 | 3–4 days |
| **Total** | **21** | **8.5–12 days** |

---

## Critical Debt Items

| ID | Description | Effort | Impact | Affected Files |
|----|-------------|--------|--------|----------------|
| TD-BKG-001 | **Cancellation entity not persisted.** `CancelBookingCommandHandler` creates a `BookingCancellation` entity in memory but never adds it to any repository. The cancellation record is silently lost — no audit trail for cancellations. | 0.5 day | No cancellation audit trail | `CancelBookingCommandHandler.cs:53-63` |
| TD-BKG-002 | **Reschedule entity not persisted.** `RescheduleBookingCommandHandler` creates a `BookingReschedule` entity but never adds it to any repository. Reschedule history is silently lost. | 0.5 day | No reschedule audit trail | `RescheduleBookingCommandHandler.cs:70-87` |
| TD-BKG-003 | **Approval entity not persisted.** `BookingApprovalService.CreateApprovalRequestAsync` creates a `BookingApproval` entity and returns it, but never adds it to a repository. The `ApproveBookingCommandHandler` calls this service but the approval record is not saved. | 0.5 day | No approval audit trail | `BookingApprovalService.cs:24-46`, `ApproveBookingCommandHandler.cs:28-44` |
| TD-BKG-004 | **Hardcoded secrets in configuration.** JWT signing key (`REPLACE-WITH-A-SECURE-SECRET-KEY-AT-LEAST-32-CHARS-LONG!!`) and database credentials (`postgres/postgres`) are in `appsettings.json`. | 1 day | Full system compromise if repo is compromised | `appsettings.json:10,17` |

---

## Medium Debt Items

| ID | Description | Effort | Impact | Affected Files |
|----|-------------|--------|--------|----------------|
| TD-BKG-005 | **`HandleFailure` duplicated across 8 controllers.** Identical string-matching logic to map error messages to HTTP status codes. If any handler error message changes, the mapped status silently changes. | 1 day | Fragile error-to-status mapping | All booking controllers |
| TD-BKG-006 | **`MapToDto` is `internal static` on `CreateBookingCommandHandler`** and called by 15+ other handlers across different namespaces. Tight coupling — a DTO mapping change requires recompilation of all dependent handlers. | 1 day | Maintenance burden, tight coupling | `CreateBookingCommandHandler.cs:116-145`, all command handlers |
| TD-BKG-007 | **No `[EnableRateLimiting]` on booking controllers.** Rate limit policies exist in `Program.cs` but are never applied to booking endpoints. | 0.5 day | API abuse vulnerability | All 8 booking controllers |
| TD-BKG-008 | **No `ICurrentUser` injection in command handlers.** `BookingCreatorId` is never populated from JWT claims. Commands don't know who initiated the booking. | 1 day | No user attribution on bookings | Multiple command handlers |
| TD-BKG-009 | **`GetBookingStatisticsQueryHandler` loads ALL bookings into memory** then computes aggregates via LINQ. For busy academies, this will cause high memory usage and slow responses. | 1 day | OOM risk, slow statistics | `GetBookingStatisticsQueryHandler.cs:31-84` |
| TD-BKG-010 | **`GetUpcomingBookingsQueryHandler` loads all bookings for date range** then filters by status in memory. Unnecessary data transfer. | 0.5 day | Unnecessary DB load | `GetUpcomingBookingsQueryHandler.cs:32-39` |
| TD-BKG-011 | **`GetByAthleteIdAsync` called without date filter** in `AvailabilityService` and `ConflictDetectionService` — loads entire athlete booking history. | 0.5 day | Scales poorly with history | `AvailabilityService.cs:113-114`, `ConflictDetectionService.cs:84-85` |
| TD-BKG-012 | **No audit trail population.** `BookingHistory` entity exists but no handler writes to it. `CreatedBy`/`UpdatedBy` are ignored in all configurations. | 1 day | No change audit trail | All command handlers, all configurations |
| TD-BKG-013 | **Fire-and-forget search recording.** `BookingsSearchController.AdvancedSearch` fires `RecordBookingSearchCommand` with `_ = _mediator.Send(...)` — errors are silently swallowed. | 0.5 day | Silent failure of search history | `BookingsSearchController.cs:101` |

---

## Low Debt Items

| ID | Description | Effort | Impact | Affected Files |
|----|-------------|--------|--------|----------------|
| TD-BKG-014 | **`GetUserId` helper duplicated across 5 controllers.** Identical JWT claim extraction logic. | 0.25 day | Code duplication | `BookingsController`, `BookingsSearchController`, `BookingApprovalsController`, `BookingWaitlistsController`, `BookingsCalendarController` |
| TD-BKG-015 | **`SaveBookingSearchApiRequest` defined in controller file** instead of DTOs folder. | 0.25 day | Code organization | `BookingsSearchController.cs:323-341` |
| TD-BKG-016 | **`SearchBookingsResponse` defined in controller file** instead of DTOs folder. | 0.25 day | Code organization | `BookingsController.cs:750-756` |
| TD-BKG-017 | **Inconsistent route naming.** `/api/v1/booking-statistics` (singular) vs `/api/v1/bookings` (plural). | 0.25 day | API UX inconsistency | `BookingStatisticsController.cs:21` |
| TD-BKG-018 | **Potential infinite loop in booking number generation.** `SchedulingEngine.GenerateBookingNumberAsync` loops until unique, with no max retry. | 0.25 day | Theoretical hang | `SchedulingEngine.cs:27-38` |
| TD-BKG-019 | **`SaveToken = true` in JWT configuration.** Stores JWT in `HttpContext` unnecessarily. | 0.1 day | Minor memory waste | `Program.cs:183` |
| TD-BKG-020 | **CORS localhost fallback.** When no origins configured, localhost is allowed. | 0.25 day | Security in prod | `Program.cs:140-157` |
| TD-BKG-021 | **3 child entities missing `HasQueryFilter` for soft delete.** `BookingItem`, `BookingParticipant`, `BookingReminder` return soft-deleted records. | 0.5 day | Data leak risk | 3 configuration files |

---

## Priority Order for Remediation

| Phase | Items | Rationale |
|-------|-------|-----------|
| **Phase 1 — Data Integrity** | TD-BKG-001, TD-BKG-002, TD-BKG-003, TD-BKG-021 | Audit trail gaps and soft-delete leaks |
| **Phase 2 — Security** | TD-BKG-004, TD-BKG-007, TD-BKG-020 | Secrets and rate limiting |
| **Phase 3 — User Context** | TD-BKG-008, TD-BKG-012 | User attribution and audit |
| **Phase 4 — Performance** | TD-BKG-009, TD-BKG-010, TD-BKG-011 | In-memory filtering |
| **Phase 5 — Code Quality** | TD-BKG-005, TD-BKG-006, TD-BKG-014 | DRY violations |
| **Phase 6 — Polish** | TD-BKG-013, TD-BKG-015–TD-BKG-019 | Minor improvements |

---

*This register should be reviewed and updated weekly during remediation sprints.*
