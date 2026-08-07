# Academy Feature

## Purpose
Academy discovery and management: browse/search academies, view details,
enroll, and manage curriculum for academy owners.

## Scope
- Academy listing, search, filters
- Academy details (coaches, programs, facilities)
- Enrollment and curriculum management (owner)

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
