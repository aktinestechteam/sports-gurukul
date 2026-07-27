# Competition Engine - Platform Architecture

## Overview

The Competition Engine (`SportsGurukul.Platform.Competition`) is a shared, reusable platform kernel designed to power all competition-based features across the Sports Gurukul platform. It supports Tournament Management, League Management, School Competitions, Championships, and AI Tournament Planner.

## Design Principles

- **Strategy Pattern**: Each tournament format and seeding strategy is an independently pluggable strategy
- **No switch statements for format logic**: Format-specific behavior is encapsulated in strategy classes
- **Platform-level reusability**: Not tied to any single module; consumed via DI registration
- **Immutable configuration**: `CompetitionConfig` drives all behavior; strategies are stateless

## Architecture

```
SportsGurukul.Platform.Competition/
├── Models/                    # Domain models (Participant, CompetitionMatch, Bracket, etc.)
│   └── Enums/                 # CompetitionFormat, SeedingStrategy, MatchStatus, etc.
├── Interfaces/                # Core contracts
│   ├── ICompetitionEngine     # Top-level orchestrator
│   ├── IFormatStrategy        # Tournament format strategy contract
│   ├── ISeedingStrategy       # Seeding algorithm contract
│   ├── IBracketGenerationService
│   ├── IFixtureGenerationService
│   ├── IAdvancementService
│   ├── IMatchAssignmentService
│   ├── IMatchScheduler
│   ├── IRankingCalculator
│   └── Scheduling/            # Scheduling sub-interfaces
├── Engines/
│   ├── CompetitionEngine.cs   # Orchestrator implementation
│   └── Formats/               # 7 format strategy implementations
├── Seeding/                   # 6 seeding strategy implementations
├── Services/                  # Service implementations
└── DependencyInjection.cs     # AddCompetitionEngine() extension method
```

## Supported Formats

| Format | Strategy Class | Description |
|--------|---------------|-------------|
| Single Elimination | `SingleEliminationStrategy` | Standard bracket with BYEs for top seeds. Supports optional third-place match. |
| Double Elimination | `DoubleEliminationStrategy` | Winners + Losers brackets + Grand Final with bracket reset. |
| Round Robin | `RoundRobinStrategy` | Every participant plays every other participant once. |
| Swiss System | `SwissSystemStrategy` | Pairing-based rounds where similar-record players face each other. |
| League | `LeagueStrategy` | Division-based round robin with configurable group counts. |
| Hybrid Tournament | `HybridTournamentStrategy` | Combines group stage with knockout phases. |
| Group Stage + Knockout | `GroupStageKnockoutStrategy` | Round-robin groups feeding into elimination bracket. |

## Seeding Strategies

| Strategy | Class | Description |
|----------|-------|-------------|
| Random | `RandomSeedingStrategy` | Shuffles participants randomly. |
| Ranking-Based | `RankingBasedSeedingStrategy` | Sorts by participant ranking, top seed gets advantage. |
| Manual | `ManualSeedingStrategy` | Preserves existing seed order. |
| Regional | `RegionalSeedingStrategy` | Distributes by region for balanced competition. |
| Academy-Based | `AcademyBasedSeedingStrategy` | Interleaves participants from different academies. |
| Balanced Draw | `BalancedDrawSeedingStrategy` | Ensures balanced skill distribution across bracket halves. |

## Key Models

- **Participant**: Represents a competitor with ranking, region, and academy info
- **CompetitionMatch**: A single match with home/away participants, scores, status, and winner
- **Bracket**: A collection of rounds containing matches
- **Fixture**: A scheduled match with venue, time, and court assignment
- **Seed**: Participant placement information for bracket positioning
- **Ranking**: Calculated standings with points, wins, losses, goal difference
- **CompetitionConfig**: Full configuration driving format, seeding, scoring rules, and tiebreakers
- **MatchSet**: Individual set/game scores within a match (for sports like tennis, cricket)

## DI Registration

```csharp
// In Startup/Program.cs
services.AddCompetitionEngine();
```

This registers all format strategies, seeding strategies, services, and the `ICompetitionEngine` orchestrator.

## Usage Example

```csharp
var engine = serviceProvider.GetRequiredService<ICompetitionEngine>();

var config = new CompetitionConfig
{
    TournamentId = tournamentId,
    Format = CompetitionFormat.SingleElimination,
    SeedingStrategy = SeedingStrategy.RankingBased,
    PointsForWin = 3,
    PointsForDraw = 1,
    PointsForLoss = 0,
    Tiebreakers = new List<RankingTiebreaker>
    {
        RankingTiebreaker.GoalDifference,
        RankingTiebreaker.GoalsScored,
        RankingTiebreaker.Wins
    }
};

var result = await engine.GenerateCompetitionAsync(config, participants);
// result.Brackets, result.Matches, result.Fixtures, result.Rankings
```

## Performance Characteristics

| Operation | 100 participants | 1,000 participants |
|-----------|-----------------|-------------------|
| Single Elimination generation | <1ms | <5ms |
| Round Robin generation | <50ms | N/A (large) |
| Ranking calculation (100 matches/participant) | <10ms | <200ms |
| 10K participants single elimination | ~30ms | N/A |

## Integration Points

- **Tournament Management Module**: Uses engine for bracket generation, fixture scheduling, and ranking calculations
- **League Management Module** (planned): League format with division management
- **School Competitions** (planned): Academy-based seeding with regional constraints
- **AI Tournament Planner** (planned): Automated scheduling with conflict detection
