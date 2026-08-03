# Athlete Feature

## Purpose
Athlete profiles and development: view athlete details, performance history,
and track progress across programs.

## Scope
- Athlete profile and search
- Performance/assessment history
- Program tracking

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
