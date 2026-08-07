# BACKEND_INTEGRATION

Status: **Adopted** - Owner: Chief Software Architect

## 1. State of the backend

The backend is **completed** (ASP.NET Core 9, Clean Architecture). It is not
subject to change from this repository's Flutter work.

- Repo: `backend/` (Domain / Application / Infrastructure / Api projects).
- Contract source of truth: `docs/api/openapi.yaml` (+ `docs/api/API_Specifications.md`).
- Running backend exposes Swagger UI for interactive reference.

## 2. Rules

1. **Swagger is the source of truth.** Before writing any integration code,
   open `docs/api/openapi.yaml` and use the exact paths, methods, request
   bodies, query params, headers and response shapes defined there.
2. **Never invent APIs.** If a needed endpoint is not in the contract, stop
   and ask - do not fabricate a path or a request/response shape.
3. **Never modify backend contracts.** No "small improvements" to paths,
   field names, enums or status codes. The backend owns the contract.
4. **Always consume existing endpoints** through the Dio layer
   (`NETWORKING.md`) and repositories (`ARCHITECTURE.md`).

## 3. How integration code is structured

```
Feature page (presentation)
  -> view-model / provider
    -> use case (application)
      -> repository interface (domain)
        -> repository impl (infrastructure)
          -> API datasource (Dio via ApiClient.create())
```

- DTOs (from the OpenAPI shapes) live in `infrastructure/models/` as
  `XxxDto` freezed classes, mapped to domain entities at the boundary.
- Auth headers (JWT) flow through `AuthInterceptor` (functional in P005).
- Every request carries `X-Request-Id` (see `NETWORKING.md`).

## 4. Verification

- Integration code must be verified against the actual API, not assumed:
  confirm paths/shapes from `openapi.yaml` and, where possible, against a
  running backend.
- Response contracts that don't match the OpenAPI spec are a bug in the
  mobile code (or a spec drift to flag - never silently adapt both).

## 5. Reference

- `API_GUIDELINES.md` (how to consume endpoints)
- `docs/mobile/04-API-Integration-Guide.md`
- `docs/api/openapi.yaml`
