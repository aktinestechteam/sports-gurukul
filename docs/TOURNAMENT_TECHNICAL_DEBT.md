# Tournament Module — Technical Debt Register

**Date:** 2026-07-28
**Module:** Tournament Management
**Total Items:** 42
**Estimated Total Effort:** 280–420 hours (7–10.5 developer-weeks)

---

## HIGH Priority (Must resolve before production — 15 items)

| # | Debt Item | Category | File(s) | Effort | Impact if Deferred |
|---|---|---|---|---|---|
| H1 | **5 command handlers are no-ops** — `RegenerateFixtures`, `GenerateParticipantNumbers`, `PublishResults` never persist data; `GetTournamentByIdQueryHandler` hardcodes `MatchCount=0`; `GetParticipantStatisticsQueryHandler` never populates sets/games | Correctness | Application/Features/TournamentManagement/Commands/ | 16–24h | Users cannot regenerate fixtures, get participant numbers, or view accurate tournament stats |
| H2 | **Live scoring infrastructure is in-memory only** — `MemoryMatchStore`, `RedisLiveScoreCache`, `RankingService`, `StandingsService`, `MedalService` all use `ConcurrentDictionary` with no persistence; `Scoped` services reset per request | Architecture | Platform.Competition/Services/, Application/Features/LiveScoringManagement/ | 40–60h | Live scoring completely non-functional in production; data lost on every request |
| H3 | **SignalR live update publisher is no-op** — all `Publish*` methods return `Task.CompletedTask` | Architecture | Platform.Competition/Services/SignalRLiveUpdatePublisher.cs | 8–12h | No real-time updates reach clients |
| H4 | **`TournamentRepository.IsTournamentCodeUniqueAsync` returns inverted result** — `AnyAsync` returns true when code EXISTS, callers expect true when UNIQUE | Bug | Infrastructure/Persistence/Repositories/TournamentRepository.cs:118 | 2–4h | Duplicate tournament codes allowed in production |
| H5 | **`CancelTournamentCommandHandler` sets `TournamentStatus.Archived` instead of `Cancelled`** | Bug | Application/Features/TournamentManagement/Commands/CancelTournament/CancelTournamentCommandHandler.cs:37 | 1h | Cannot cancel a tournament; conflates two lifecycle events |
| H6 | **Hardcoded secrets in committed config** — DB password `postgres` in `appsettings.json` and `docker-compose.yml`; JWT signing key placeholder | Security | appsettings.json:10,17; docker-compose.yml:33 | 4–8h | Secret exposure via source control |
| H7 | **`GetMatchById` full table scan** — sends `Guid.Empty` as `TournamentId`, loads ALL matches, LINQ filters | Bug | Api/Controllers/V1/TournamentMatchesController.cs:80 | 2–4h | O(N) per request; performance degrades with scale |
| H8 | **Potential infinite loops** — `RegionalSeedingStrategy` and `AcademyBasedSeedingStrategy` have `while (result.Count < participants.Count)` without guaranteed progress | Bug | Platform.Competition/Seeding/RegionalSeedingStrategy.cs:17; AcademyBasedSeedingStrategy.cs:20 | 4–8h | Application hangs under certain participant distributions |
| H9 | **13 command handlers lack FluentValidation** — Cancel, Archive, Publish, PublishResults, OpenRegistration, CloseRegistration, StartMatch, Reschedule, AssignCourt, AssignOfficial, AwardMedals, RecordWalkover, RecordForfeit, ApproveRegistration, RejectRegistration | Quality | Application/Features/TournamentManagement/Commands/ | 16–24h | Invalid input reaches handlers; potential security and data integrity issues |
| H10 | **`HandleFailure` string-based HTTP status routing** — controllers check `error.Contains("not found")` to map 404; fragile and non-idiomatic | Architecture | All 8 Tournament controllers | 12–16h | Error responses silently change HTTP status when handler messages change |
| H11 | **`CreatedBy`/`UpdatedBy` audit fields ignored in DB** — every configuration calls `builder.Ignore(e => e.CreatedBy)` | Data | All EF configurations | 4–6h | No audit trail; compliance violation |
| H12 | **`TournamentRanking` cascade deletes on Participant removal** — historical ranking data destroyed | Data | Infrastructure/Persistence/Configurations/TournamentRankingConfiguration.cs:44 | 2–4h | Historical rankings lost when participant is deleted |
| H13 | **No CI/CD pipeline** — no GitHub Actions, no Azure DevOps, no automation | DevOps | N/A | 16–24h | Manual builds; no quality gates; no deployment automation |
| H14 | **Docker runs as root with no health checks** — security and reliability gap | DevOps | backend/Dockerfile | 4–6h | Container security vulnerability; orchestrator cannot detect unhealthy containers |
| H15 | **Stub domain services** — `IScoringService`, `IRankingCalculationService`, `IFixtureGenerationService`, `ISeedingService`, `IBracketGenerationService` all return empty/trivial results | Architecture | Application/Features/TournamentManagement/Services/ | 40–60h | No real bracket generation, fixture scheduling, ranking calculation, or scoring logic |

---

## MEDIUM Priority (Should resolve within 2 sprints — 18 items)

| # | Debt Item | Category | File(s) | Effort | Impact if Deferred |
|---|---|---|---|---|---|
| M1 | **Anemic Domain Model** — all 24 entities are pure POCOs with zero business logic | Architecture | All Domain/Entities/ | 40–60h | Domain rules enforced only in handlers; inconsistent enforcement |
| M2 | **Pervasive denormalization** — Name strings duplicated alongside FK references in 6+ entities | Data | TournamentMatch, Registration, Participant, Fixture, Award | 16–24h | Stale names when source entity is updated |
| M3 | **`TournamentResult` duplicates `TournamentMatch`** — scores/winners exist on both | Data | Domain/Entities/TournamentResult.cs | 8–12h | Two sources of truth; update inconsistency risk |
| M4 | **Polymorphic FKs without discriminator** — `AthleteId/TeamId/AcademyId` with no mutual exclusion | Data | TournamentRegistration, TournamentParticipant | 8–12h | Multiple FKs can be set simultaneously |
| M5 | **Code duplication: `MapToDto`** — static method in `CreateTournamentCommandHandler` referenced by 6 handlers | Quality | Application/Features/TournamentManagement/Commands/ | 4–6h | Change in mapping requires updating multiple files |
| M6 | **Code duplication: `HandleFailure`** — identical string-matching logic in 8 controllers | Quality | All Tournament controllers | 4–6h | Bug fixes require 8 separate edits |
| M7 | **Code duplication: `SeedParticipants`** — identical method in 5 Competition Engine format strategies | Quality | Platform.Competition/Engines/Formats/ | 4–6h | Changes require 5 edits |
| M8 | **No `pageSize` upper bound** — search endpoints accept unbounded page sizes | Security | Tournament search controllers | 2–4h | DoS via large page size |
| M9 | **No `[ValidateAntiForgeryToken]` on mutation endpoints** | Security | All controllers | 4–6h | CSRF vulnerability if cookie auth is used |
| M10 | **`TournamentRanking` has sport-specific fields** — `SetsWon/SetsLost/GamesWon/GamesLost` are tennis-specific | Architecture | Domain/Entities/TournamentRanking.cs | 8–12h | Overfits to specific sport; breaks for non-racket sports |
| M11 | **`InMemoryAsyncQueryProvider<T>` duplicated** in 2 test files | Quality | Application.Tests/QueryHandlers/ | 2–4h | Maintenance burden |
| M12 | **`SeedPosition` unique per tournament, not per category** — multi-category tournaments cannot share seed positions | Data | TournamentSeedConfiguration.cs:30 | 2–4h | Multi-category tournaments break |
| M13 | **`MatchNumber` unique per tournament, not per stage** | Data | TournamentMatchConfiguration.cs:62 | 2–4h | Multi-stage tournaments break |
| M14 | **`Swagger route conflict` on `/awards`** — (fixed in source, needs rebuild) | Bug | TournamentResultsController.cs | 1h | Swagger generation crashes |
| M15 | **`LeaderboardService` returns null always** | Architecture | Platform.Competition/Services/LeaderboardService.cs | 4–8h | No leaderboard functionality |
| M16 | **`FixtureGenerationService` hardcodes `TournamentId = Guid.Empty`** | Bug | Platform.Competition/Services/FixtureGenerationService.cs:26 | 2–4h | Fixtures not linkable to tournaments |
| M17 | **`AdvancementService` incorrect bracket advancement** — hardcoded formulas don't work for all bracket sizes | Bug | Platform.Competition/Services/AdvancementService.cs:46 | 8–12h | Wrong bracket progression in elimination formats |
| M18 | **`DoubleEliminationStrategy` losers bracket placeholder** — empty matches without participant assignment | Bug | Platform.Competition/Engines/Formats/DoubleEliminationStrategy.cs:102 | 12–16h | Double elimination format partially broken |

---

## LOW Priority (Backlog — 9 items)

| # | Debt Item | Category | File(s) | Effort | Impact if Deferred |
|---|---|---|---|---|---|
| L1 | **`RowVersion` repeated in every entity** — should be on `BaseEntity` | DRY | All Domain/Entities/ | 4–6h | Code duplication |
| L2 | **`TournamentCourt` uses `FacilityCourtStatus`** — enum from different bounded context | Naming | Domain/Entities/TournamentCourt.cs | 1–2h | Semantic confusion |
| L3 | **`MedalType` enum defined but never used** | Dead code | Domain/Enums/MedalType.cs | 0.5h | Unused code |
| L4 | **`Rules_` trailing underscore naming** | Naming | Domain/Entities/Tournament.cs | 1h | Code smell |
| L5 | **`IsPublished` redundant with `Status = Published`** | Data | Domain/Entities/Tournament.cs | 2h | Two sources of truth |
| L6 | **`ApiVersion("1.0")` on every controller** — should be global default | Config | All controllers | 1–2h | Repetitive |
| L7 | **`TournamentSport.SportName` denormalized from Sport entity** | Data | Domain/Entities/TournamentSport.cs | 1–2h | Stale data risk |
| L8 | **Storage services are stubs** — S3 and Azure Blob never actually upload | Architecture | Infrastructure/Services/S3StorageService.cs, AzureBlobStorageService.cs | 16–24h | File uploads are non-functional |
| L9 | **Integration test dead code** — `PostgreSqlContainerFixture` and possibly `TestAuthHandler` unused | Quality | Tournament.IntegrationTests/ | 2h | Confusing for new developers |

---

## Effort Summary

| Priority | Items | Low Est. (hours) | High Est. (hours) |
|---|---|---|---|
| HIGH | 15 | 178 | 272 |
| MEDIUM | 18 | 84 | 132 |
| LOW | 9 | 27 | 43 |
| **TOTAL** | **42** | **289** | **447** |

**Estimated total: 289–447 hours (7.2–11.2 developer-weeks)**

---

## Recommended Prioritization

### Sprint 1 (Week 1–2): Critical Blockers
- H1: Fix no-op handlers (16–24h)
- H4: Fix inverted unique code check (2–4h)
- H5: Fix Cancel → Archived bug (1h)
- H6: Externalize secrets (4–8h)
- H7: Fix GetMatchById full table scan (2–4h)
- H14: Docker hardening (4–6h)

### Sprint 2 (Week 3–4): Live Scoring + Security
- H2: Implement DB-backed live scoring (40–60h)
- H3: Implement SignalR publisher (8–12h)
- H8: Fix infinite loop bugs (4–8h)
- H9: Add missing validators (16–24h)
- M8: Add pagination bounds (2–4h)

### Sprint 3 (Week 5–6): Architecture + DevOps
- H10: Replace string-based error routing (12–16h)
- H11: Persist audit fields (4–6h)
- H12: Fix cascade delete (2–4h)
- H13: Set up CI/CD pipeline (16–24h)
- H15: Begin implementing domain services

### Sprint 4 (Week 7–8): Domain + Quality
- M1: Begin enriching domain model
- M2–M4: Address denormalization
- M5–M7: Extract shared code
- M10: Refactor sport-specific ranking fields

---

*Generated 2026-07-28. Re-evaluate after each sprint.*
