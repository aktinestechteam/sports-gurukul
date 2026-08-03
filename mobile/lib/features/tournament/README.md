# Tournament Feature

## Purpose
Tournament discovery and participation: browse tournaments, register
teams/players, and follow brackets and results.

## Scope
- Tournament listing and details
- Team/player registration
- Bracket and result viewing

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
