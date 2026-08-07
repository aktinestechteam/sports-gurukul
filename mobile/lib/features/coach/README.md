# Coach Feature

## Purpose
Coach profiles and management: view coach details, availability, specializations,
and manage coaching engagements.

## Scope
- Coach search and profile
- Availability and specialization display
- Engagement/assignment management

## Structure
```
presentation/      Pages, widgets, providers
application/       Use cases and services (orchestration)
domain/            Entities, repository interfaces, value objects, failures
infrastructure/    Datasources, DTO models, repository impls, mappers
```

## Status
Scaffolded in P002. No business implementation yet.

## Rules
- Presentation never calls APIs/databases directly.
- DTOs never leave infrastructure.
- No cross-feature imports (share via core/shared services only).
