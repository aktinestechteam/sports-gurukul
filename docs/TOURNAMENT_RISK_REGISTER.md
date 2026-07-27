# Tournament Module — Risk Register

**Date:** 2026-07-28
**Module:** Tournament Management
**Total Risks:** 20

---

## Risk Matrix

| | Impact: Low | Impact: Medium | Impact: High | Impact: Critical |
|---|---|---|---|---|
| **Likelihood: High** | R12 | R8, R16 | R1, R3 | R5, R7 |
| **Likelihood: Medium** | R15, R20 | R4, R9, R11 | R2, R6 | R10 |
| **Likelihood: Low** | R17 | R13, R14, R18 | R19 | — |

---

## Risk Details

### R1: Live Scoring Data Loss on Every Request
| Field | Value |
|---|---|
| **ID** | R1 |
| **Description** | `RankingService`, `StandingsService`, and `MedalService` use `ConcurrentDictionary` in Scoped DI. Each HTTP request creates a new scope, resetting all live scoring data to empty. `MemoryMatchStore` is Singleton but never persists to DB — server restart loses everything. |
| **Likelihood** | High |
| **Impact** | Critical |
| **Mitigation** | Replace in-memory stores with PostgreSQL-backed persistence + Redis caching layer. Implement proper repository pattern for live scoring data. |
| **Owner** | Backend Team |
| **Target Sprint** | Sprint 2 |
| **Status** | OPEN |

### R2: Unbounded Memory Growth (OOM)
| Field | Value |
|---|---|
| **ID** | R2 |
| **Description** | `MemoryMatchStore` (Singleton) and `RedisLiveScoreCache` (Singleton) use `ConcurrentDictionary` with no max-size cap, no TTL eviction, and unbounded `ScoreEvents` list per match. Under sustained traffic, memory grows without limit until `OutOfMemoryException`. |
| **Likelihood** | Medium |
| **Impact** | High |
| **Mitigation** | Implement LRU eviction policy or use Redis with TTL. Cap `ScoreEvents` per match. Add memory monitoring alerts. |
| **Owner** | Backend Team |
| **Target Sprint** | Sprint 2 |
| **Status** | OPEN |

### R3: No-Op Command Handlers
| Field | Value |
|---|---|
| **ID** | R3 |
| **Description** | `RegenerateFixturesCommandHandler`, `GenerateParticipantNumbersCommandHandler`, and `PublishResultsCommandHandler` accept commands, perform validation, but never persist any changes. Users believe operations succeeded but data is unchanged. |
| **Likelihood** | High |
| **Impact** | High |
| **Mitigation** | Implement proper persistence in each handler. Add integration tests verifying DB state after command execution. |
| **Owner** | Backend Team |
| **Target Sprint** | Sprint 1 |
| **Status** | OPEN |

### R4: Duplicate Tournament Codes
| Field | Value |
|---|---|
| **ID** | R4 |
| **Description** | `IsTournamentCodeUniqueAsync` returns `true` when the code EXISTS (inverted logic). `CreateTournamentCommandHandler` generates sequential codes (`TRN-{timestamp}`) without checking uniqueness. Two tournaments created in the same second get identical codes. |
| **Likelihood** | Medium |
| **Impact** | Medium |
| **Mitigation** | Fix inverted logic. Add uniqueness check before creation. Add unique index on `TournamentCode` (already exists but bypassed by inverted check). |
| **Owner** | Backend Team |
| **Target Sprint** | Sprint 1 |
| **Status** | OPEN |

### R5: Hardcoded Secrets in Source Control
| Field | Value |
|---|---|
| **ID** | R5 |
| **Description** | Database password `postgres` committed in `appsettings.json` and `docker-compose.yml`. JWT signing key placeholder committed. Any repository compromise exposes database and token signing. |
| **Likelihood** | High |
| **Impact** | Critical |
| **Mitigation** | Move to Azure Key Vault / AWS Secrets Manager. Use User Secrets for development. Add `.gitignore` entries. Rotate all committed secrets. |
| **Owner** | DevOps / Security |
| **Target Sprint** | Sprint 1 |
| **Status** | OPEN |

### R6: SQL Injection (Not Present — Low Risk)
| Field | Value |
|---|---|
| **ID** | R6 |
| **Description** | All database access uses EF Core with parameterized queries. No raw SQL found. Risk is LOW but must be monitored as codebase evolves. |
| **Likelihood** | Low |
| **Impact** | High |
| **Mitigation** | Continue using EF Core. Add code review policy禁止 raw SQL. Enable SAST scanning in CI/CD. |
| **Owner** | Backend Team |
| **Target Sprint** | N/A |
| **Status** | MONITORING |

### R7: Application Hang (Infinite Loops)
| Field | Value |
|---|---|
| **ID** | R7 |
| **Description** | `RegionalSeedingStrategy` and `AcademyBasedSeedingStrategy` have `while (result.Count < participants.Count)` loops without guaranteed progress when groups are uneven. Can spin indefinitely. |
| **Likelihood** | High |
| **Impact** | Critical |
| **Mitigation** | Add max-iteration guard. Refactor loop logic to guarantee progress. Add timeout. |
| **Owner** | Backend Team |
| **Target Sprint** | Sprint 2 |
| **Status** | OPEN |

### R8: No CI/CD Pipeline
| Field | Value |
|---|---|
| **ID** | R8 |
| **Description** | No GitHub Actions, Azure DevOps, or any CI/CD automation exists. All builds and deployments are manual. No quality gates enforce test passing, code coverage, or security scanning. |
| **Likelihood** | High |
| **Impact** | Medium |
| **Mitigation** | Implement GitHub Actions pipeline with build, test, security scan, Docker build, staging deploy, production approval. |
| **Owner** | DevOps |
| **Target Sprint** | Sprint 3 |
| **Status** | OPEN |

### R9: Stale Denormalized Data
| Field | Value |
|---|---|
| **ID** | R9 |
| **Description** | `HomeParticipantName`, `AwayParticipantName`, `WinnerName`, `RegistrantName`, `ParticipantName` stored as strings alongside FK references. If source entity name changes, these become stale. |
| **Likelihood** | Medium |
| **Impact** | Medium |
| **Mitigation** | Remove denormalized name fields. Use navigation properties for display names. Add computed columns if needed for search. |
| **Owner** | Backend Team |
| **Target Sprint** | Sprint 4 |
| **Status** | OPEN |

### R10: CSRF Vulnerability
| Field | Value |
|---|---|
| **ID** | R10 |
| **Description** | No `[ValidateAntiForgeryToken]` on mutation endpoints. If cookie-based auth is ever used alongside JWT, state-changing operations are vulnerable to CSRF attacks. Current JWT-only auth partially mitigates this. |
| **Likelihood** | Low |
| **Impact** | Critical |
| **Mitigation** | Add `[ValidateAntiForgeryToken]` or configure CSRF policy. Document that JWT-only auth is the only supported mode. |
| **Owner** | Security / Backend Team |
| **Target Sprint** | Sprint 2 |
| **Status** | OPEN |

### R11: Unbounded Query Results (DoS)
| Field | Value |
|---|---|
| **ID** | R11 |
| **Description** | Search endpoints accept `pageSize` without `[Range]` upper bound. A client can request `pageSize=1000000` to dump entire tables, exhausting memory and DB connections. |
| **Likelihood** | Medium |
| **Impact** | Medium |
| **Mitigation** | Add `[Range(1, 100)]` to `pageSize` parameter. Add server-side maximum cap. |
| **Owner** | Backend Team |
| **Target Sprint** | Sprint 2 |
| **Status** | OPEN |

### R12: Missing Audit Trail
| Field | Value |
|---|---|
| **ID** | R12 |
| **Description** | `CreatedBy`/`UpdatedBy` fields exist in domain but are `Ignore`d in all EF configurations. No record of who created or modified any tournament data. |
| **Likelihood** | High |
| **Impact** | Low |
| **Mitigation** | Remove `Ignore()` calls. Implement `ICurrentUserService` to populate audit fields automatically via SaveChanges interceptor. |
| **Owner** | Backend Team |
| **Target Sprint** | Sprint 3 |
| **Status** | OPEN |

### R13: Docker Container Runs as Root
| Field | Value |
|---|---|
| **ID** | R13 |
| **Description** | `Dockerfile` has no `USER` directive. API container runs as root, violating container security best practices. |
| **Likelihood** | Low |
| **Impact** | Medium |
| **Mitigation** | Add `RUN adduser --disabled-password --gecos "" appuser` and `USER appuser` to Dockerfile. |
| **Owner** | DevOps |
| **Target Sprint** | Sprint 1 |
| **Status** | OPEN |

### R14: No Container Health Checks
| Field | Value |
|---|---|
| **ID** | R14 |
| **Description** | No `HEALTHCHECK` in Dockerfile, no health check endpoint in API. Orchestrator (Kubernetes/Docker Compose) cannot detect unhealthy containers. |
| **Likelihood** | Low |
| **Impact** | Medium |
| **Mitigation** | Add `/health` and `/health/ready` endpoints. Add `HEALTHCHECK` to Dockerfile. Configure health checks in docker-compose. |
| **Owner** | Backend / DevOps |
| **Target Sprint** | Sprint 3 |
| **Status** | OPEN |

### R15: `TournamentRanking` Cascade Delete
| Field | Value |
|---|---|
| **ID** | R15 |
| **Description** | Deleting a `TournamentParticipant` cascades to delete all associated `TournamentRanking` records. Historical rankings are permanent data and should survive participant deletion. |
| **Likelihood** | Low |
| **Impact** | Low |
| **Mitigation** | Change `OnDelete(DeleteBehavior.Cascade)` to `DeleteBehavior.Restrict` or `SetNull` for ranking-participant FK. |
| **Owner** | Backend Team |
| **Target Sprint** | Sprint 3 |
| **Status** | OPEN |

### R16: No Rate Limiting Applied
| Field | Value |
|---|---|
| **ID** | R16 |
| **Description** | Rate limiter policies ("auth", "sensitive", "default") are defined in `Program.cs` but no `[EnableRateLimiting(...)]` attribute exists on any controller. All endpoints are unprotected. |
| **Likelihood** | High |
| **Impact** | Medium |
| **Mitigation** | Add `[EnableRateLimiting("default")]` to all controllers. Add `[EnableRateLimiting("sensitive")]` to auth and mutation endpoints. |
| **Owner** | Backend Team |
| **Target Sprint** | Sprint 2 |
| **Status** | OPEN |

### R17: Double Elimination Losers Bracket Broken
| Field | Value |
|---|---|
| **ID** | R17 |
| **Description** | `DoubleEliminationStrategy` creates empty `CompetitionMatch` objects in the losers bracket without participant assignment. The `droppedFromWinners` list is built but never used. |
| **Likelihood** | Low |
| **Impact** | Low |
| **Mitigation** | Implement proper losers bracket population logic. Add test cases for 4, 8, 16 participant double elimination. |
| **Owner** | Backend Team |
| **Target Sprint** | Sprint 4 |
| **Status** | OPEN |

### R18: Swiss System Returns Empty Matches
| Field | Value |
|---|---|
| **ID** | R18 |
| **Description** | `SwissSystemStrategy.GenerateNextRoundAsync` performs pairing logic but returns `new List<CompetitionMatch>()` instead of the generated matches. |
| **Likelihood** | Low |
| **Impact** | Medium |
| **Mitigation** | Return the paired matches instead of discarding them. |
| **Owner** | Backend Team |
| **Target Sprint** | Sprint 4 |
| **Status** | OPEN |

### R19: No Distributed Tracing / Observability
| Field | Value |
|---|---|
| **ID** | R19 |
| **Description** | No OpenTelemetry, no correlation IDs, no structured request logging, no custom metrics. Cannot diagnose issues in production. |
| **Likelihood** | Low |
| **Impact** | High |
| **Mitigation** | Add OpenTelemetry SDK. Implement correlation ID middleware. Add structured logging with Serilog. Create Grafana dashboards. |
| **Owner** | Backend / DevOps |
| **Target Sprint** | Sprint 3 |
| **Status** | OPEN |

### R20: Storage Services Are Stubs
| Field | Value |
|---|---|
| **ID** | R20 |
| **Description** | `S3StorageService` and `AzureBlobStorageService` have TODO comments and return fake URLs without actually uploading files. `LocalStorageService` works but is not production-suitable. |
| **Likelihood** | Medium |
| **Impact** | Low |
| **Mitigation** | Implement actual S3/Azure Blob upload. Add integration tests with MinAzurite. Remove TODO stubs. |
| **Owner** | Backend Team |
| **Target Sprint** | Backlog |
| **Status** | OPEN |

---

## Risk Summary

| Severity | Count | Items |
|---|---|---|
| Critical | 3 | R1, R5, R7, R10 |
| High | 5 | R2, R3, R6, R19 |
| Medium | 7 | R4, R8, R9, R11, R13, R14, R16, R18 |
| Low | 5 | R12, R15, R17, R20 |

---

*Generated 2026-07-28. Review monthly and update status.*
