# Feature Development Guide

Status: **Adopted** · Owner: Mobile Architecture Team

## 1. Before You Start

1. Read `docs/01-Architecture-Overview.md` and `docs/02-Folder-Structure.md`.
2. Confirm the backend contract in the Swagger/OpenAPI spec (endpoints,
   DTO shapes, auth requirements) — do not redesign existing APIs.
3. Create a feature branch; keep the change scoped to the feature.

## 2. Feature Scaffold

Every feature lives under `lib/features/<feature>/` with four layers:

```
features/<feature>/
├── presentation/        Pages, widgets, providers
│   ├── pages/           Route targets
│   ├── widgets/         Feature-local widgets
│   └── providers/       Riverpod providers for this feature
├── application/
│   ├── usecases/        Single-responsibility use cases
│   └── services/        Cross-use-case orchestration services
├── domain/
│   ├── entities/        Pure domain models (no Flutter/HTTP/JSON)
│   ├── repositories/    Abstract repository interfaces
│   ├── value_objects/   Typed primitives with invariants
│   └── failures/        Feature `Failure` types (see 07-Coding-Standards)
└── infrastructure/
    ├── datasources/     Remote/local data access
    ├── models/          DTOs; map <-> domain entities
    ├── mappers/         DTO <-> entity mappers
    └── repositories/    `XxxRepositoryImpl` implementations
```

Also update or create the feature `README.md` describing purpose, scope
and status.

## 3. Dependency Flow

```
Presentation → Application → Domain ← Infrastructure
```

- Domain is the innermost, dependency-free core of the feature.
- Application orchestrates; it knows domain + repository interfaces only.
- Infrastructure implements the interfaces and knows the outside world.
- Presentation depends only on application/domain abstractions.

## 4. Wiring with Riverpod

1. Define the abstract repository in `domain/repositories/`.
2. Implement it in `infrastructure/repositories/`.
3. Expose providers (repository → datasource, then use case/view-model
   providers) in `presentation/providers/`.
4. Pages consume providers; never construct repositories directly.

## 5. Integration Checklist

- [ ] No hardcoded strings (all from ARB, `flutter gen-l10n` run).
- [ ] No hardcoded colors/spacing — design tokens only.
- [ ] Route added to `app/router/` (names + paths centralized) and guarded
      if role-specific.
- [ ] Error/empty/loading states handled on async screens.
- [ ] Backend contract honoured; DTOs mapped to domain at the boundary.
- [ ] Tests added under `test/features/` (widget/unit as appropriate).
- [ ] `dart format` clean, `flutter analyze` clean, `flutter test` green.

## 6. Definition of Done

All checklist items pass and the feature works on the app shell
(`flutter run`). No cross-feature imports were introduced; anything
shared was promoted to `core/` or `shared/`.
