# AI Feature

## Purpose
AI-powered assistance (planned): practice suggestions, skill assessments,
and intelligent recommendations surfaced across the app.

## Scope
- AI coach / practice recommendations
- Skill assessment insights
- Recommendation surfaces

## Structure
```
presentation/      Pages, widgets, providers
application/       Use cases and services (orchestration)
domain/            Entities, repository interfaces, value objects, failures
infrastructure/    Datasources, DTO models, repository impls, mappers
```

## Status
Scaffolded in P002. Backend AI endpoints exist (see Swagger); integration
is a later sprint.

## Rules
- Presentation never calls APIs/databases directly.
- DTOs never leave infrastructure.
- No cross-feature imports (share via core/shared services only).
