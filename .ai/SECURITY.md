# SECURITY

Status: **Adopted** - Owner: Chief Software Architect

Security rules for the mobile app. Product-level detail:
`docs/mobile/08-Platform/06-Security-&-Compliance.md`.

## 1. Secrets & keys

- **Never commit secrets, API keys, tokens, or credentials** (code or docs).
- Inject secrets via environment configuration; store runtime secrets in
  `SecureStorage` (`lib/core/storage/secure_storage.dart`, platform keychain).
- Non-sensitive settings go to `PreferenceStorage`; the two never share a
  key namespace and never cross over.

## 2. Authentication (JWT)

- JWT handling lands with auth (P005+): store access + refresh tokens in
  `SecureStorage`, inject via `AuthInterceptor`, refresh centrally, and treat
  401s through a single auth flow - never per-screen.
- Tokens are never logged, never placed in shared_preferences, and never
  embedded in URLs.

## 3. Input validation

- Validate and sanitize all user input (forms) before it reaches the API;
  match backend validation rules (lengths, formats, ranges).
- Treat all remote data as untrusted: validate/sanitize before rendering.

## 4. Transport & pinning

- HTTPS only. No plain-HTTP endpoints in any environment config.
- **Certificate pinning is a future item** (tracked in `TECH_DEBT.md`);
  do not ship pinning half-configured.

## 5. PII protection

- Minimize PII in logs, analytics and error reports.
- `LoggingInterceptor` logs method/URI/status **only** - never headers,
  payloads, tokens, or query strings containing PII.
- `AppLogger` is the only logging path; `print()` is banned.

## 6. App-level hardening

- No debug backdoors; debug-only behaviour gated behind environment checks.
- Do not cache sensitive data in Drift; use `SecureStorage`.
- Review mobile API keys for third-party services at integration time
  (see `REVIEW_CHECKLIST.md`).

## 7. Reference

- `docs/mobile/06-Authentication/06-Biometric-Authentication.md`
- `docs/mobile/08-Platform/06-Security-&-Compliance.md`
- `NETWORKING.md` (logging rules)
