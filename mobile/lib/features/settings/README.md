# Settings Feature

## Purpose
Application settings: theme, language, notifications, privacy, and app
preferences. Owns the persistence of user-level preferences.

## Scope
- Theme (light/dark/system) and language selection
- Notification and privacy toggles
- App info / version

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
