# Dashboard Feature

## Purpose
Post-login landing screen. Aggregates the user's context: upcoming
academies/bookings, announcements, quick actions, and coach/athlete
role-specific tiles.

## Scope
- Role-aware home shell entry
- Upcoming bookings / academy summary cards
- Quick action shortcuts

## Structure
```
presentation/      Pages, widgets, providers
application/       Use cases and services (orchestration)
domain/            Entities, repository interfaces, value objects, failures
infrastructure/    Datasources, DTO models, repository impls, mappers
```

## Status
Scaffolded in P002. `dashboard_page.dart` is a placeholder landing page.

## Rules
- Presentation never calls APIs/databases directly.
- DTOs never leave infrastructure.
- No cross-feature imports (share via core/shared services only).
