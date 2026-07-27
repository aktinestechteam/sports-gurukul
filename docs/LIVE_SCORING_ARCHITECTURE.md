# Live Match Management & Ranking System — Architecture

## Overview

The Live Match Management & Ranking System is a **platform-level capability** designed to provide real-time scoring, lifecycle management, rankings, leaderboards, standings, medal tables, and statistics for any sports competition. It is implemented as a reusable layer that serves Tournaments, Leagues, Championships, School Sports, Inter Academy Competitions, and can be extended for AI-powered analytics.

## Design Principles

- **Platform Reusable**: All abstractions and implementations live in `SportsGurukul.Platform.Competition` — no tournament-specific logic.
- **Sport Agnostic**: `ISportRuleProvider` allows sport-specific scoring rules without modifying core services.
- **Real-time Abstraction**: `ILiveUpdatePublisher`/`ILiveUpdateSubscriber` define pub/sub contracts without binding to any transport (SignalR, gRPC, etc.).
- **Cache Abstraction**: `ILiveScoreCache` abstracts Redis (or any distributed cache) — `MemoryMatchStore` is used as the in-memory default.
- **CQRS**: Application layer uses Command/Query separation via MediatR.
- **Strategy Pattern**: Sport-specific rules are pluggable via `ISportRuleProvider` — no switch statements on sport codes.
- **Result Pattern**: All operations return `Result<T>` for consistent error handling.

## Architecture Layers

```
┌─────────────────────────────────────────────────────────────┐
│                       API Layer                              │
│  LiveScoringController (17 endpoints)                       │
├─────────────────────────────────────────────────────────────┤
│                  Application Layer                           │
│  Commands (10) · Queries (6) · DTOs (6) · Validators (16)  │
├─────────────────────────────────────────────────────────────┤
│              Platform.Competition Layer                       │
│  Interfaces (11) · Services (8) · Sport Providers (6)      │
│  Models (15+) · Enums (4)                                   │
├─────────────────────────────────────────────────────────────┤
│                     Domain Layer                             │
│  Enums (4): LiveMatchStatus, ScoringUnit,                   │
│             LeaderboardType, MedalType                       │
└─────────────────────────────────────────────────────────────┘
```

## Platform Layer (`SportsGurukul.Platform.Competition`)

### Interfaces

| Interface | Responsibility |
|---|---|
| `ILiveScoringService` | Start match, update score, undo score, get live match state |
| `IMatchLifecycleService` | State machine: Scheduled→CheckIn→WarmUp→Live↔Paused→Completed/Walkover/Forfeit/Abandoned/Cancelled |
| `IRankingService` | Calculate rankings, update after match |
| `IStandingsService` | Tournament standings (league table) |
| `IMedalService` | Medal table generation and awarding |
| `ILeaderboardService` | Leaderboard generation by type (Tournament, Academy, Coach, Athlete, Sport, Season) |
| `IStatisticsService` | Match, player, and team statistics |
| `ISportRuleProvider` | Sport-specific scoring rules (goals, sets, runs, points, etc.) |
| `ILiveUpdatePublisher` | Publish real-time updates (SignalR, etc.) |
| `ILiveUpdateSubscriber` | Subscribe to real-time updates |
| `ILiveScoreCache` | Distributed cache abstraction for live scores |

### Services

| Service | Description |
|---|---|
| `LiveScoringService` | Core scoring logic, manages `MemoryMatchStore` |
| `MatchLifecycleService` | State machine transitions with valid-transition validation |
| `RankingService` | Point-based ranking calculation |
| `StandingsService` | League-style standings (W/L/D, GF/GA/GD) |
| `MedalService` | Gold/Silver/Bronze medal tracking |
| `LeaderboardService` | Multi-type leaderboard generation |
| `StatisticsService` | Match and player statistics aggregation |
| `MemoryMatchStore` | `ConcurrentDictionary`-based in-memory match store |

### Sport Rule Providers

| Provider | Scoring Rules |
|---|---|
| `FootballSportRuleProvider` | Goals (1 point each), 2 halves, 90 min |
| `CricketSportRuleProvider` | Runs, wickets, overs, innings |
| `BadmintonSportRuleProvider` | Points to 21, best of 3 sets |
| `ChessSportRuleProvider` | Win/Loss/Draw, time control |
| `AthleticsSportRuleProvider` | Time-based (fastest wins), laps |
| `SwimmingSportRuleProvider` | Time-based (fastest wins), strokes |

### Live Match Lifecycle

```
Scheduled ──→ CheckInOpen ──→ WarmUp ──→ Live
                                       ↕
                                     Paused
                                       │
                ┌──────────────────────┘
                ↓
            Completed
            Walkover
            Forfeit
            Abandoned
            Cancelled
```

### Models

- `LiveMatch`: Core live match state (score, events, participants, metadata)
- `MatchScore`: Total points, games, sets, periods, breakdown
- `LiveScoreEvent`: Individual scoring event with undo support
- `MatchStatistics` / `PlayerStatistics` / `TeamStatistics`: Aggregated stats
- `Leaderboard` / `LeaderboardEntry`: Multi-type leaderboard
- `MedalTable` / `MedalEntry`: Medal tracking
- `StandingsEntry`: League table entry
- `SportScoringConfig`: Sport-specific configuration

## Application Layer (CQRS)

### Commands (10)

| Command | Description |
|---|---|
| `StartLiveMatchCommand` | Start a live match from scheduled state |
| `PauseMatchCommand` | Pause an active match |
| `ResumeMatchCommand` | Resume a paused match |
| `UpdateLiveScoreCommand` | Update score + auto-update rankings & standings |
| `UndoScoreCommand` | Undo last score event |
| `CompleteMatchCommand` | Complete a match with optional winner |
| `RecordWalkoverCommand` | Record a walkover result |
| `RecordForfeitCommand` | Record a forfeit result |
| `PublishResultsCommand` | Publish final results |
| `GenerateLeaderboardCommand` | Generate a leaderboard for a tournament |

### Queries (6)

| Query | Description |
|---|---|
| `LiveScoreQuery` | Get real-time score for a match |
| `LeaderboardQuery` | Get leaderboard by type |
| `TournamentStandingsQuery` | Get tournament standings |
| `MedalTableQuery` | Get medal table |
| `MatchStatisticsQuery` | Get match statistics |
| `PlayerStatisticsQuery` | Get player statistics across tournament |

### Validators (16)

All commands and queries have FluentValidation validators enforcing:
- Non-empty GUIDs for IDs
- Positive values for scores and period numbers
- Maximum length constraints on descriptions and names
- Required fields

### DTOs (6 files)

| DTO | Purpose |
|---|---|
| `LiveScoreDto` | Live score response with events |
| `LeaderboardDto` | Leaderboard with entries |
| `StandingsDto` | League standings |
| `MedalTableDto` | Medal counts per participant |
| `MatchStatisticsDto` | Match-level statistics |
| `PlayerStatisticsDto` | Player-level statistics |

## API Layer

### LiveScoringController (17 endpoints)

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/v1/live/matches/{id}/start` | Coach+ | Start live match |
| POST | `/api/v1/live/matches/{id}/pause` | Coach+ | Pause match |
| POST | `/api/v1/live/matches/{id}/resume` | Coach+ | Resume match |
| POST | `/api/v1/live/matches/{id}/score` | Coach+ | Update score |
| POST | `/api/v1/live/matches/{id}/undo` | Coach+ | Undo score |
| POST | `/api/v1/live/matches/{id}/complete` | Manager+ | Complete match |
| POST | `/api/v1/live/matches/{id}/walkover` | Manager+ | Record walkover |
| POST | `/api/v1/live/matches/{id}/forfeit` | Manager+ | Record forfeit |
| GET | `/api/v1/live/matches/{id}/score` | Anonymous | Get live score |
| GET | `/api/v1/live/matches/{id}/statistics` | Anonymous | Get match stats |
| GET | `/api/v1/live/tournaments/{id}/leaderboard` | Anonymous | Get leaderboard |
| POST | `/api/v1/live/tournaments/{id}/leaderboard` | Manager+ | Generate leaderboard |
| GET | `/api/v1/live/tournaments/{id}/standings` | Anonymous | Get standings |
| GET | `/api/v1/live/tournaments/{id}/medals` | Anonymous | Get medal table |
| GET | `/api/v1/live/tournaments/{id}/players/{pid}/statistics` | Anonymous | Get player stats |
| POST | `/api/v1/live/tournaments/{id}/matches/{mid}/publish` | Manager+ | Publish results |

## Testing

### Competition Engine Tests (39)
- Format strategies (Single/Double Elimination, Round Robin, Swiss, Ladder, Pyramid, Group Stage)
- Seeding strategies
- Ranking calculator
- Performance benchmarks

### Live Scoring Validator Tests (26)
- All 10 command validators tested
- All 6 query validators tested
- Valid/invalid input scenarios
- Edge cases (empty GUIDs, negative scores, zero periods)

## Future Extensions

1. **Real-time Transport**: Implement `ILiveUpdatePublisher`/`ILiveUpdateSubscriber` with SignalR
2. **Redis Cache**: Implement `ILiveScoreCache` with Redis for production
3. **AI Analytics**: Player performance predictions, tactical insights, injury risk assessment
4. **School Sports**: Age-group-specific rules, handicap systems
5. **Inter Academy**: Cross-academy rankings, rivalry tracking
6. **Mobile Push**: Push notifications for score updates via FCM/APNs
