# PROJECT_CONTEXT

Status: **Current as of P004** - Owner: Chief Software Architect

> This is the canonical project context. Read this first. Every other `.ai`
> document builds on this file.

## Project Vision

Sports Gurukul is an **AI-first digital sports ecosystem** connecting
athletes, coaches, academies, parents, scouts, sponsors and tournament
organizers on a unified platform. The tagline is **"Train x Compete x Excel"**.

## Goals

- One platform that serves five role types from a single mobile codebase:
  **Athlete**, **Parent**, **Coach**, **Academy**, **Super Admin**.
- Role-based experiences with offline-first behaviour (training, attendance,
  performance, tournaments, payments, AI coaching).
- AI assistance (AI Coach) layered on top of a conventional sports platform.
- A monorepo with a completed backend, a growing Flutter app, a React admin
  portal and Python AI services.

## Modules

| Module | Owner | Status |
| --- | --- | --- |
| `backend/` | ASP.NET Core 9 (Clean Architecture) | **Completed** |
| `ai-services/` | FastAPI + LangGraph + RAG | Completed / extending |
| `mobile/` | Flutter (this project) | Sprint 0 complete; feature work pending |
| `web-admin/` | React 19 + Vite + TypeScript | Active |
| `docs/` | Architecture, PRD, API specs | Living |

## Technology Stack (mobile)

| Concern | Choice |
| --- | --- |
| Language / UI | Dart / Flutter 3.44 (Material 3) |
| Architecture | Clean Architecture + Feature First |
| DI / State | Riverpod 3.x (**only**; no Provider/Bloc/GetX) |
| Navigation | go_router ^17 (centralized routes + guards) |
| HTTP | Dio ^5.11 (interceptor chain, retries, request IDs) |
| Local DB | Drift ^2.34 (offline-first, migrations) |
| Models | freezed 3.2.5 + json_serializable (build_runner) |
| Storage | flutter_secure_storage (secrets) + shared_preferences (prefs) |
| Networking utils | connectivity_plus, package_info_plus, device_info_plus, uuid |
| Logging | package:logger behind `AppLogger` facade; `print()` banned |
| Linting | very_good_analysis (strict); `flutter analyze` zero issues |
| Localization | flutter_localizations + gen_l10n (**en, hi, mr**) |
| Tests | flutter_test + mocktail; built-in `matchesGoldenFile` goldens |

## Backend Status

The backend is **complete and is not to be redesigned**. `docs/api/openapi.yaml`
is the contract source of truth. The Flutter app consumes existing endpoints
only and must never invent or modify API contracts (see
`BACKEND_INTEGRATION.md` and `API_GUIDELINES.md`).

## Flutter Status

- **P001** - Project foundation: architecture, folder structure, theming,
  localization, routing, CI-ready tooling.
- **P002** - Bootstrap verified: app boots splash -> placeholder dashboard;
  Riverpod + go_router wired; test baseline green.
- **P003** - Engineering foundations: strict linting (very_good_analysis),
  freezed/json codegen, dio network layer with interceptors, drift scaffold,
  storage facades, utilities, testing tooling, engineering docs.
- **P004** - AI development governance & knowledge base (this `.ai/` dir).
- **Next (P005+)** - Real feature development: authentication, onboarding,
  then role-based modules. `sample_model.dart` is removed when real models
  land. The `AuthInterceptor` placeholder becomes functional.

## Future Roadmap

1. Auth + onboarding (JWT in secure storage, biometrics).
2. Role-based dashboards and route guards.
3. Offline-first outbox (`OfflineQueue` table in Drift) + conflict resolution.
4. AI Coach integration with `ai-services/`.
5. CI pipeline (format/analyze/test on PRs) and code coverage gates.
6. Certificate pinning, deep links, analytics.

## Reference

- Mobile product/technical specs: `docs/mobile/` (authoritative).
- Mobile sprint docs: `mobile/docs/` (numbered 01-13).
- API contract: `docs/api/openapi.yaml`, `docs/api/API_Specifications.md`.
