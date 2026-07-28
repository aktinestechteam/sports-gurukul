using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventSearchDiscovery.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Engines;

public class RecommendationEngine : IRecommendationEngine
{
    private readonly IEventSearchRepository _searchRepository;
    private readonly IEnumerable<IRecommendationStrategy> _strategies;
    private readonly ILogger<RecommendationEngine> _logger;

    public RecommendationEngine(
        IEventSearchRepository searchRepository,
        IEnumerable<IRecommendationStrategy> strategies,
        ILogger<RecommendationEngine> logger)
    {
        _searchRepository = searchRepository;
        _strategies = strategies;
        _logger = logger;
    }

    public async Task<IReadOnlyList<RecommendationDto>> GetRecommendationsAsync(
        Guid? userId, string? userCity, IReadOnlyList<string> preferredSports,
        IReadOnlyList<string> preferredEventTypes, decimal? latitude, decimal? longitude,
        int limit, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting recommendations for user {UserId}, city {City}", userId, userCity);

        var events = await _searchRepository.GetUpcomingEventsAsync(limit * 3, DateTime.UtcNow, cancellationToken);
        if (events.Count == 0) return [];

        var scored = new List<(Domain.Entities.Event evt, double score, string reason)>();

        foreach (var strategy in _strategies.OrderBy(s => s.Priority))
        {
            var scores = await strategy.ScoreEventsAsync(events, userId, preferredSports, preferredEventTypes, cancellationToken);
            foreach (var s in scores)
            {
                var evt = events.FirstOrDefault(e => e.Id == s.EventId);
                if (evt is not null)
                {
                    scored.Add((evt, s.Score, s.Reason));
                }
            }
        }

        return scored
            .GroupBy(x => x.evt.Id)
            .Select(g => new { FirstEvent = g.First().evt, AvgScore = g.Average(x => x.score), Reasons = string.Join("; ", g.Select(x => x.reason).Distinct()) })
            .OrderByDescending(x => x.AvgScore)
            .Take(limit)
            .Select(x => MapToRecommendationDto(x.FirstEvent, x.AvgScore, x.Reasons))
            .ToList();
    }

    public async Task<IReadOnlyList<RecommendationDto>> GetPersonalizedRecommendationsAsync(
        Guid userId, int limit, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting personalized recommendations for user {UserId}", userId);
        return await GetRecommendationsAsync(userId, null, [], [], null, null, limit, cancellationToken);
    }

    public async Task<IReadOnlyList<TrendingEventDto>> GetTrendingEventsAsync(
        string? city, int limit, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting trending events for city {City}", city);
        var events = await _searchRepository.GetTrendingEventsAsync(limit, cancellationToken);

        return events.Select(e => new TrendingEventDto
        {
            Id = e.Id,
            EventName = e.EventName,
            EventCode = e.EventCode,
            BannerUrl = e.BannerUrl,
            EventType = e.EventType?.ToString(),
            StartDate = e.StartDate,
            AcademyName = string.Empty,
            ViewCount = 0,
            RegistrationCount = 0,
            AverageRating = 0,
            TrendingScore = 0
        }).ToList();
    }

    public async Task<IReadOnlyList<FeaturedEventDto>> GetFeaturedEventsAsync(
        string? city, string? sportName, int limit, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting featured events for city {City}, sport {Sport}", city, sportName);
        var events = await _searchRepository.GetFeaturedEventsAsync(limit, cancellationToken);

        return events.Select(e => new FeaturedEventDto
        {
            Id = e.Id,
            EventName = e.EventName,
            EventCode = e.EventCode,
            ShortDescription = e.ShortDescription,
            BannerUrl = e.BannerUrl,
            EventType = e.EventType?.ToString(),
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            RegistrationFee = e.RegistrationFee,
            AcademyName = string.Empty,
            AverageRating = 0,
            RegistrationCount = 0,
            MaxParticipants = e.MaxParticipants,
            Priority = e.IsFeatured ? 1 : 0
        }).ToList();
    }

    private static RecommendationDto MapToRecommendationDto(Domain.Entities.Event evt, double score, string reason)
    {
        return new RecommendationDto
        {
            Id = evt.Id,
            EventName = evt.EventName,
            EventCode = evt.EventCode,
            ShortDescription = evt.ShortDescription,
            BannerUrl = evt.BannerUrl,
            EventType = evt.EventType?.ToString(),
            StartDate = evt.StartDate,
            RegistrationFee = evt.RegistrationFee,
            AcademyName = string.Empty,
            RelevanceScore = score,
            RecommendationReason = reason
        };
    }
}
