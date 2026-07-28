using SportsGurukul.Application.Features.EventSearchDiscovery.DTOs;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Engines;

public interface IRecommendationEngine
{
    Task<IReadOnlyList<RecommendationDto>> GetRecommendationsAsync(
        Guid? userId, string? userCity, IReadOnlyList<string> preferredSports,
        IReadOnlyList<string> preferredEventTypes, decimal? latitude, decimal? longitude,
        int limit, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RecommendationDto>> GetPersonalizedRecommendationsAsync(
        Guid userId, int limit, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TrendingEventDto>> GetTrendingEventsAsync(
        string? city, int limit, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FeaturedEventDto>> GetFeaturedEventsAsync(
        string? city, string? sportName, int limit, CancellationToken cancellationToken = default);
}
