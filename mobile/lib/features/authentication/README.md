# Authentication Feature

## Purpose
Handles all identity concerns: sign in, OTP verification, forgot password,
session management, refresh tokens, and biometric/PIN unlock (planned).

## Scope
- Welcome / Login / OTP / Forgot-password flows
- Session state and token lifecycle
- Role assignment used by role-based navigation

## Structure
```
presentation/      Pages, widgets, providers
application/       Use cases and services (orchestration)
domain/            Entities, repository interfaces, value objects, failures
infrastructure/    Datasources, DTO models, repository impls, mappers
```

## Status
Scaffolded in P002. No business implementation yet; authentication APIs
already exist in the backend (see Swagger) and will be consumed in a later
sprint without redesigning them.

## Rules
- Presentation never calls APIs/databases directly.
- DTOs never leave infrastructure.
- No cross-feature imports (share via core/shared services only).
