using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Engines;

public interface IRecommendationStrategy
{
    string StrategyName { get; }
    int Priority { get; }

    Task<IReadOnlyList<EventScore>> ScoreEventsAsync(
        IReadOnlyList<Event> events, Guid? userId,
        IReadOnlyList<string> preferredSports,
        IReadOnlyList<string> preferredEventTypes,
        CancellationToken cancellationToken = default);
}

public class EventScore
{
    public Guid EventId { get; set; }
    public double Score { get; set; }
    public string Reason { get; set; } = string.Empty;
}
