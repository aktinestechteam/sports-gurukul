# Risk Register — Communication Platform

> **Project:** SportsGurukul Communication Module
> **Last Updated:** 2026-07-30
> **Total Risks:** 8
> **High:** 2 | **Medium:** 5 | **Low:** 1

---

## Risk Scoring Matrix

| | Impact: Low | Impact: Medium | Impact: High | Impact: Critical |
|---|---|---|---|---|
| **Probability: Certain** | Medium | High | Critical | Critical |
| **Probability: High** | Low | Medium | High | Critical |
| **Probability: Medium** | Low | Medium | Medium | High |
| **Probability: Low** | Low | Low | Medium | Medium |

---

## High Risks

| Risk ID | Description | Probability | Impact | Score | Mitigation Strategy | Owner | Status |
|---|---|---|---|---|---|---|---|
| R-001 | **Data loss from missing `SaveChangesAsync`.** All notification services (NotificationService, TemplateService, PreferenceService, CampaignService) modify entities via `Repository.Update()` but never call `SaveChangesAsync()`. In production with EF Core + PostgreSQL, no data is persisted. Every create, update, cancel, mute, and retry operation is silently discarded. | Certain | Critical | Critical | Add `IUnitOfWork` dependency and `SaveChangesAsync` calls to all 4 notification services. Add integration tests that verify data persistence against a real database. | Backend Team | Open |
| R-002 | **Secrets hardcoded in configuration.** JWT signing key uses placeholder value (`"REPLACE-WITH-A-SECURE-SECRET-KEY..."`), database connection string uses `postgres:postgres`, SMTP credentials empty. Secrets exposed in version control history and deployment artifacts. | Medium | Critical | High | Externalize all secrets to Azure Key Vault or environment variables. Rotate credentials immediately. Disable Swagger in production. | DevOps | Open |

---

## Medium Risks

| Risk ID | Description | Probability | Impact | Score | Mitigation Strategy | Owner | Status |
|---|---|---|---|---|---|---|---|
| R-003 | **Dormant Communication Platform library.** `SportsGurukul.Platform.Communication` is fully built but not wired into the API. Real provider delivery, queue processing, circuit breaker, and template rendering are not used. | High | High | High | Add project reference to `SportsGurukul.Api.csproj`, call `AddCommunicationPlatform()` in `Program.cs`, replace stub services with real implementations. | Backend Team | Open |
| R-004 | **Campaign data lost on restart.** CampaignService stores campaigns in an in-memory `ConcurrentDictionary` instead of the database. All campaign data is lost on application restart or scale-out. | High | Medium | Medium | Implement `ICampaignRepository` with EF Core persistence. Add database migration for `NotificationCampaign` entity. | Backend Team | Open |
| R-005 | **Silent HTTP status code changes from string-matching error handling.** `HandleFailure` in 6 controllers maps error messages to HTTP status codes using string comparison. Any message change silently alters the returned status code, breaking API consumers. | High | Low | Medium | Implement typed error codes or an enum-based error system. Return structured error responses with machine-readable codes. Extract shared `HandleFailure` to a base controller. | Backend Team | Open |
| R-006 | **Observability gaps for production debugging.** No structured request logging, no correlation IDs, no distributed tracing, no APM integration. Diagnosing production issues across services requires manual correlation. | Medium | Medium | Medium | Add request logging middleware with correlation ID generation. Integrate OpenTelemetry (traces, metrics, logs). Configure log aggregation (Elasticsearch/Splunk/Application Insights). | Backend Team | Open |
| R-007 | **API abuse from insufficient rate limiting.** Only the `default` rate limit policy (100 req/min) is applied to communication controllers. No per-user or per-endpoint differentiation. Sensitive endpoints (retry, reprocess) share the same limits as read endpoints. | Medium | Medium | Medium | Implement tiered rate limiting: strict for mutate operations (10 req/min), moderate for list/search (30 req/min), standard for read (100 req/min). Add per-user rate limiting based on JWT claims. | Backend Team | Open |

---

## Low Risks

| Risk ID | Description | Probability | Impact | Score | Mitigation Strategy | Owner | Status |
|---|---|---|---|---|---|---|---|
| R-008 | **Unbounded search results on notification endpoints.** Search endpoint defaults to page size 20 but lacks a maximum cap. Malicious or accidental large page requests could cause database load spikes. | Low | Medium | Low | Add server-enforced maximum page size (e.g., 100). Validate page number to prevent deep pagination abuse. | Backend Team | Open |

---

## Risk Summary

| Category | Count | Critical | High | Medium | Low |
|---|---|---|---|---|---|
| Data Integrity | 2 | 1 | 0 | 1 | 0 |
| Security | 1 | 0 | 1 | 0 | 0 |
| Integration | 1 | 0 | 1 | 0 | 0 |
| Observability | 1 | 0 | 0 | 1 | 0 |
| API Quality | 2 | 0 | 0 | 1 | 1 |
| Performance | 1 | 0 | 0 | 0 | 1 |
| **Total** | **8** | **1** | **2** | **3** | **2** |

---

## Remediation Timeline

| Phase | Risks | Target | Dependencies |
|---|---|---|---|
| **Phase 1 — Ship Blockers** | R-001, R-002 | Before any deployment | None |
| **Phase 2 — Integration** | R-003 | Week 1 | Phase 1 |
| **Phase 3 — Persistence** | R-004 | Week 1 | Phase 1 |
| **Phase 4 — Hardening** | R-005, R-007 | Week 2 | None |
| **Phase 5 — Observability** | R-006 | Week 3 | None |
| **Phase 6 — Polish** | R-008 | Week 3 | None |

---

*This register should be reviewed weekly. Risks that have been mitigated should be marked as **Closed** with the date and verification method.*
