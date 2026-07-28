using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Engines;

public class EventScoringEngine : IRecommendationStrategy
{
    private readonly ILogger<EventScoringEngine> _logger;

    public string StrategyName => "EventAttributeScoring";
    public int Priority => 1;

    public EventScoringEngine(ILogger<EventScoringEngine> logger)
    {
        _logger = logger;
    }

    public Task<IReadOnlyList<EventScore>> ScoreEventsAsync(
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

            if (evt.IsFeatured)
            {
                score += 30;
                reasons.Add("Featured event");
            }

            if (evt.Status == Domain.Enums.EventStatus.RegistrationOpen)
            {
                score += 20;
                reasons.Add("Registration open");
            }

            var daysUntilStart = (evt.StartDate - DateTime.UtcNow).Days;
            if (daysUntilStart is > 0 and <= 30)
            {
                score += 15;
                reasons.Add("Starting soon");
            }

            if (evt.RegistrationFee == 0 || !evt.RegistrationFee.HasValue)
            {
                score += 10;
                reasons.Add("Free event");
            }

            if (evt.MaxParticipants.HasValue)
            {
                var remaining = evt.MaxParticipants.Value;
                if (remaining <= 10)
                {
                    score += 15;
                    reasons.Add("Almost sold out");
                }
            }

            if (evt.IsPublic)
            {
                score += 5;
            }

            scores.Add(new EventScore
            {
                EventId = evt.Id,
                Score = score,
                Reason = reasons.Count > 0 ? string.Join("; ", reasons) : "General event"
            });
        }

        _logger.LogInformation("Scored {Count} events using {Strategy}", scores.Count, StrategyName);
        return Task.FromResult<IReadOnlyList<EventScore>>(scores);
    }
}
