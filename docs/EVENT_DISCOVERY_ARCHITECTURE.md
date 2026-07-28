# Event Discovery Architecture

## Overview

The Event Search & Discovery Platform is the common discovery engine for the Sports Gurukul platform, supporting Events, Training Programs, Tournaments, Academies, Coaches, and Sports Facilities. It follows Clean Architecture with CQRS pattern using MediatR.

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                       API Layer                             │
│  EventSearchController (REST endpoints)                     │
│  Swagger Examples, API Versioning, Rate Limiting            │
├─────────────────────────────────────────────────────────────┤
│                    Application Layer                        │
│  ┌──────────┐  ┌──────────┐  ┌──────────────────────────┐  │
│  │ Commands │  │ Queries  │  │ Engines (Recommendation) │  │
│  │ SaveSearch│  │ Search   │  │ RecommendationEngine     │  │
│  │ DelSearch│  │ Nearby   │  │ EventScoringStrategy     │  │
│  │ TrackView│  │ Trending │  │ PopularityScoringStrategy│  │
│  └──────────┘  │ Featured │  │ PersonalizationService   │  │
│                │ Calendar │  └──────────────────────────┘  │
│                │ Autocomplete│                              │
│                └──────────┘                                │
│  DTOs: EventCardDto, CalendarEventDto, RecommendationDto   │
│  Validators: FluentValidation for all commands/queries      │
├─────────────────────────────────────────────────────────────┤
│                  Infrastructure Layer                       │
│  EventSearchRepository (PostgreSQL via EF Core)            │
│  SearchCacheService (Redis/IDistributedCache abstraction)  │
│  EF Configurations for EventSavedSearch, EventRecentSearch │
└─────────────────────────────────────────────────────────────┘
```

## Search Flow

```
Client Request → Controller → MediatR → Query Handler → Repository → PostgreSQL
                                     ↓
                              Cache Check (Redis)
                                     ↓
                              Response DTO → API Response
```

1. **Global Search**: Full-text across event name, code, description, and tags
2. **Advanced Search**: 20+ filter parameters with dynamic query building
3. **Nearby Search**: Haversine formula for distance calculation
4. **Autocomplete**: Prefix matching with cached results (5-min TTL)
5. **Calendar Views**: Date-range queries with view-type transformation

## Recommendation Flow

```
Request → RecommendationEngine → Strategy Pipeline → Score Aggregation → Ranked Results
                    ↓
         PersonalizationService → User Preferences
                    ↓
         IRecommendationStrategy[] (EventScoring, PopularityScoring)
```

### Scoring Strategies

| Strategy | Priority | Scoring Logic |
|----------|----------|---------------|
| EventScoringEngine | 1 | Featured (+30), Registration Open (+20), Starting Soon (+15), Free (+10), Almost Sold Out (+15), Public (+5) |
| PopularityScoringEngine | 2 | High View Count (+25), Moderate Views (+15), Paid/Committed (+10) |

### Extension Points

- `IRecommendationStrategy`: Implement new scoring strategies (e.g., ML-based, collaborative filtering)
- `IRecommendationEngine`: Override entire recommendation pipeline
- `IPersonalizationService`: Replace with AI-powered personalization

## Caching Strategy

| Cache Key Pattern | TTL | Purpose |
|-------------------|-----|---------|
| `trending_events_{city}_{limit}` | 15 min | Trending events per city |
| `featured_events_{city}_{sport}_{limit}` | 15 min | Featured events |
| `recommended_events_{userId}_{limit}` | 30 min | Personalized recommendations |
| `autocomplete_{prefix}_{limit}` | 5 min | Search suggestions |
| `user_preferences_{userId}` | 24 hours | User preferences |
| `user_interactions_{userId}` | 7 days | Interaction history |

### Cache Invalidation

- Automatic TTL-based expiry
- Prefix-based bulk invalidation via `RemoveByPrefixAsync`
- Command handlers invalidate related caches on data mutations

## CQRS Commands

| Command | Handler | Description |
|---------|---------|-------------|
| `SaveSearchCommand` | `SaveSearchCommandHandler` | Persists user search filters |
| `DeleteSavedSearchCommand` | `DeleteSavedSearchCommandHandler` | Removes saved search |
| `TrackRecentlyViewedCommand` | `TrackRecentlyViewedCommandHandler` | Records event view for analytics |

## CQRS Queries

| Query | Handler | Cache | Target Performance |
|-------|---------|-------|--------------------|
| `SearchEventsQuery` | `SearchEventsQueryHandler` | None (real-time) | < 200ms |
| `UpcomingEventsQuery` | `UpcomingEventsQueryHandler` | None | < 200ms |
| `TrendingEventsQuery` | `TrendingEventsQueryHandler` | 15 min | < 150ms |
| `FeaturedEventsQuery` | `FeaturedEventsQueryHandler` | 15 min | < 150ms |
| `RecommendedEventsQuery` | `RecommendedEventsQueryHandler` | 30 min | < 150ms |
| `NearbyEventsQuery` | `NearbyEventsQueryHandler` | None | < 200ms |
| `CalendarEventsQuery` | `CalendarEventsQueryHandler` | None | < 200ms |
| `AutocompleteQuery` | `AutocompleteQueryHandler` | 5 min | < 50ms |

## Search Filters

All filters are optional and composable:

- **Sport**: Filter by sport ID
- **Academy**: Filter by academy ID
- **Coach**: Filter by coach association
- **Speaker**: Filter by speaker name
- **Venue**: Filter by venue name
- **Location**: City, State, Country
- **Date Range**: DateFrom, DateTo
- **Time Range**: TimeFrom, TimeTo
- **Price**: MinPrice, MaxPrice
- **Event Type**: Camp, Workshop, Seminar, etc.
- **Category**: SportsTraining, Education, Networking, etc.
- **Skill Level**: Beginner, Intermediate, Advanced
- **Age Group**: Age ranges
- **Availability**: Open spots
- **Registration Status**: Open, Closed, Upcoming
- **Rating**: Minimum average rating
- **Language**: Event language

## Sorting Options

| Sort Option | Behavior |
|-------------|----------|
| Upcoming (default) | By start date ascending |
| Popularity | By registration count descending |
| Recently Added | By creation date descending |
| Highest Rated | By feedback count descending |
| Alphabetical | By event name |
| Registration Closing Soon | By registration close date ascending |
| Price | By registration fee |

## Geolocation

- **Haversine Formula**: Accurate distance calculation using latitude/longitude
- **Primary Venue Priority**: Uses primary venue for distance computation
- **Radius Search**: Configurable radius (0.1 - 500 km)
- **Location Ranking**: Results sorted by proximity

## Authorization

| Endpoint | Access Level |
|----------|-------------|
| Search, Upcoming, Trending, Featured, Nearby, Calendar | Anonymous |
| Autocomplete | Anonymous |
| Track View | Anonymous (optional auth for user tracking) |
| Recommended | Anonymous (generic) / Authenticated (personalized) |
| Save/Delete Search | Authenticated |

## Performance Targets

| Operation | Target | Measurement |
|-----------|--------|-------------|
| Search | < 200ms | DB query + mapping |
| Autocomplete | < 50ms | Cache-first with fallback |
| Recommendations | < 150ms | Scored with caching |
| Nearby Search | < 200ms | In-memory distance calculation |

## Files Created

### Domain Layer
- `Domain/Entities/EventSavedSearch.cs` - Saved search entity
- `Domain/Entities/EventRecentSearch.cs` - Recent search entity

### Application Layer
- `Application/Features/EventSearchDiscovery/Commands/SaveSearch/` - Save search CQRS
- `Application/Features/EventSearchDiscovery/Commands/DeleteSavedSearch/` - Delete saved search CQRS
- `Application/Features/EventSearchDiscovery/Commands/TrackRecentlyViewed/` - Track view CQRS
- `Application/Features/EventSearchDiscovery/Queries/SearchEvents/` - Advanced search CQRS
- `Application/Features/EventSearchDiscovery/Queries/UpcomingEvents/` - Upcoming events CQRS
- `Application/Features/EventSearchDiscovery/Queries/TrendingEvents/` - Trending events CQRS
- `Application/Features/EventSearchDiscovery/Queries/FeaturedEvents/` - Featured events CQRS
- `Application/Features/EventSearchDiscovery/Queries/RecommendedEvents/` - Recommendations CQRS
- `Application/Features/EventSearchDiscovery/Queries/NearbyEvents/` - Nearby search CQRS
- `Application/Features/EventSearchDiscovery/Queries/CalendarEvents/` - Calendar view CQRS
- `Application/Features/EventSearchDiscovery/Queries/Autocomplete/` - Autocomplete CQRS
- `Application/Features/EventSearchDiscovery/Engines/` - Recommendation engines
- `Application/Features/EventSearchDiscovery/DTOs/` - Data transfer objects
- `Application/Features/EventSearchDiscovery/Validators/` - FluentValidation validators

### Infrastructure Layer
- `Infrastructure/Persistence/Repositories/EventSearchRepository.cs` - Search repository
- `Infrastructure/Caching/SearchCacheService.cs` - Redis-backed cache service
- `Infrastructure/Persistence/Configurations/EventSavedSearchConfiguration.cs` - EF config
- `Infrastructure/Persistence/Configurations/EventRecentSearchConfiguration.cs` - EF config

### API Layer
- `Api/Controllers/V1/EventSearchController.cs` - REST endpoints
- `Api/Common/Models/SwaggerExamples/EventSearchExamples.cs` - Swagger examples

### Tests
- `Application.Tests/Services/EventSearchDiscovery/` - Unit tests
- `Application.Tests/Performance/EventSearchPerformanceTests.cs` - Performance benchmarks
