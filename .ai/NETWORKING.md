# NETWORKING

Status: **Adopted** - Owner: Chief Software Architect

## 1. Rule

**Dio is the only HTTP client.** No `http` package, no direct sockets.

Current: `dio ^5.11.0`. All instances are created through
`ApiClient.create()` (`lib/core/network/api_client.dart`).

## 2. Client construction

`ApiClient.create({String baseUrl})` builds Dio with:

- `BaseOptions`: connect/receive/send timeouts (see `NetworkConfig`),
  `Accept: application/json`.
- Interceptor chain, in order:
  1. `RequestIdInterceptor` - attaches `X-Request-Id` (correlation id, uuid)
     to every request.
  2. `AuthInterceptor` - injects JWT from `SecureStorage` (placeholder today;
     functional in P005 with auth).
  3. `LoggingInterceptor` - logs method/URI/status only.
  4. `RetryInterceptor` - bounded retries via async `onError` using
     `_dio.fetch`, tracking `retry_attempt` in `options.extra`.

Base URL resolution lands with environment configuration
(`app/config/environment.dart` + `app_config.dart`).

## 3. Timeouts & retries

- Timeouts are configured centrally in `NetworkConfig`
  (`lib/core/network/network_config.dart`).
- `RetryInterceptor` retries: connection errors, timeouts, HTTP 429 and 5xx,
  up to `NetworkConfig.maxRetries`, with backoff.
- Callers never implement their own retry loops.

## 4. Error handling

- Map Dio failures at the boundary with `mapNetworkError`
  (`lib/core/network/error_mapper.dart`) -> `NetworkErrorKind`:
  `timeout`, `connectivity`, `unauthorized`, `forbidden`, `notFound`,
  `client`, `server`, `cancelled`, `unknown`.
- Repositories map `NetworkErrorKind` to feature `Failure` types; widgets
  receive typed failures, never raw exceptions.

## 5. Request IDs & tracing

- Every request carries `X-Request-Id` (header name in `NetworkConfig`) for
  end-to-end tracing with the backend.
- Preserve the request id in retries; generate once per logical request.

## 6. Logging rules (AppLogger)

- Log method, URI, status, duration. **Never log** headers, payloads, tokens,
  passwords, PII, or full stack traces that leak secrets.
- `print()` is banned; all logging through `AppLogger`
  (`lib/core/logging/app_logger.dart`).

## 7. Reference

- `docs/mobile/04-API-Integration-Guide.md`
- `docs/mobile/09-Implementation/04-Dio-API-Architecture.md`
- `mobile/docs/13-PackageDecisionLog.md` (dio decision)
