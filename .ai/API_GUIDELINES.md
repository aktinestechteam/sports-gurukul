# API_GUIDELINES

Status: **Adopted** - Owner: Chief Software Architect

Consumption rules for the completed backend API. Read after
`BACKEND_INTEGRATION.md`.

## 1. Contract discipline

- Use only endpoints defined in `docs/api/openapi.yaml`.
- Copy exact field names/types into `XxxDto` freezed classes; never rename
  contract fields (map to domain entities instead).
- Handle optional/nullable fields as declared by the spec - no assumptions
  that a field is always present.

## 2. Request conventions

- Base URL comes from environment config (`app/config/`).
- Every request gets `X-Request-Id` via `RequestIdInterceptor`.
- Auth: JWT via `AuthInterceptor` from `SecureStorage` (functional in P005).
- Content-Type: `application/json`; `Accept: application/json`.

## 3. DTO mapping

- DTOs live in `infrastructure/models/`; mappers convert DTO <-> domain entity
  at the repository boundary. DTOs never leave infrastructure.

## 4. Errors

- All failures map through `mapNetworkError` -> `NetworkErrorKind`:
  `timeout`, `connectivity`, `unauthorized`, `forbidden`, `notFound`,
  `client`, `server`, `cancelled`, `unknown`.
- Repositories translate to feature `Failure` types; UI shows friendly
  messages with retry. 401 handling (refresh/relogin) is centralized in the
  auth flow - not per screen.

## 5. Pagination & lists

- Consume list endpoints using the pagination shape defined in the OpenAPI
  spec (page/limit or cursor). Never load unbounded lists into memory -
  use `ListView.builder` + pagination (see `PERFORMANCE.md`).

## 6. Idempotency & retries

- `RetryInterceptor` retries timeouts/connectivity/429/5xx with backoff;
  do not add per-call retries.
- For mutation endpoints, reuse the request id / client mutation id if the
  spec defines one, so retries are safe against duplicates.

## 7. Versioning & deprecation

- Respect the API version scheme in the spec (path/query/header versioning).
- Deprecated fields are not consumed; keep an eye on the spec changelog.

## 8. Reference

- `docs/mobile/04-API-Integration-Guide.md`
- `docs/mobile/09-Implementation/04-Dio-API-Architecture.md`
- `NETWORKING.md`
