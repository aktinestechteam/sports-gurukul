# Booking Feature

## Purpose
Session and slot booking: browse available slots, book sessions, manage
bookings, and process cancellations/reschedules.

## Scope
- Slot availability and booking flow
- Booking history and details
- Cancel/reschedule flows

## Structure
```
presentation/      Pages, widgets, providers
application/       Use cases and services (orchestration)
domain/            Entities, repository interfaces, value objects, failures
infrastructure/    Datasources, DTO models, repository impls, mappers
```

## Status
Scaffolded in P002. No business implementation yet; backend APIs exist
(see Swagger) and will be consumed without redesign.

## Rules
- Presentation never calls APIs/databases directly.
- DTOs never leave infrastructure.
- No cross-feature imports (share via core/shared services only).
