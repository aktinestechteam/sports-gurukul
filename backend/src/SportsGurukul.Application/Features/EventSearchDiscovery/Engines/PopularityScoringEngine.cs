using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Engines;

public class PopularityScoringEngine : IRecommendationStrategy
{
    private readonly IEventSearchRepository _searchRepository;
    private readonly ILogger<PopularityScoringEngine> _logger;

    public string StrategyName => "PopularityScoring";
    public int Priority => 2;

    public PopularityScoringEngine(IEventSearchRepository searchRepository, ILogger<PopularityScoringEngine> logger)
    {
        _searchRepository = searchRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<EventScore>> ScoreEventsAsync(
        IReadOnlyList<Event> events, Guid? userId,
        IReadOnlyList<string> preferredSports,
        IReadOnlyList<string> preferredEventTypes,
        CancellationToken cancellationToken = default)
    {
        var scores = new List<EventScore>();

        foreach (var evt in events)
        {
            double score = 0;
            var reasons = new List<string>();

            var viewCount = await _searchRepository.GetViewCountAsync(evt.Id, cancellationToken);
            if (viewCount > 100)
            {
                score += 25;
                reasons.Add("High view count");
            }
            else if (viewCount > 50)
            {
                score += 15;
                reasons.Add("Moderate view count");
            }

            if (evt.RegistrationType == Domain.Enums.EventRegistrationType.Paid)
            {
                score += 10;
                reasons.Add("Paid event (committed)");
            }

            scores.Add(new EventScore
            {
                EventId = evt.Id,
                Score = score,
                Reason = reasons.Count > 0 ? string.Join("; ", reasons) : "Low popularity"
            });
        }

        _logger.LogInformation("Scored {Count} events using {Strategy}", scores.Count, StrategyName);
        return scores;
    }
}
