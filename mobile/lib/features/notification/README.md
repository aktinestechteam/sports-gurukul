# Notification Feature

## Purpose
In-app notifications and alerts: notification center, unread badge state,
and notification preferences.

## Scope
- Notification list and read/unread state
- Badge counts on the navigation shell
- Notification preferences

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
