using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.Competition.Interfaces;
using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Services;

public class MatchLifecycleService : IMatchLifecycleService
{
    private readonly MemoryMatchStore _store;
    private readonly ILiveUpdatePublisher _publisher;
    private readonly ILogger<MatchLifecycleService> _logger;

    private static readonly Dictionary<LiveMatchStatus, HashSet<LiveMatchStatus>> ValidTransitions = new()
    {
        [LiveMatchStatus.Scheduled] = new() { LiveMatchStatus.CheckInOpen },
        [LiveMatchStatus.CheckInOpen] = new() { LiveMatchStatus.WarmUp, LiveMatchStatus.Cancelled },
        [LiveMatchStatus.WarmUp] = new() { LiveMatchStatus.Live, LiveMatchStatus.Cancelled },
        [LiveMatchStatus.Live] = new() { LiveMatchStatus.Paused, LiveMatchStatus.Completed, LiveMatchStatus.Walkover, LiveMatchStatus.Forfeit, LiveMatchStatus.Abandoned },
        [LiveMatchStatus.Paused] = new() { LiveMatchStatus.Live, LiveMatchStatus.Cancelled, LiveMatchStatus.Abandoned },
    };

    public MatchLifecycleService(MemoryMatchStore store, ILiveUpdatePublisher publisher, ILogger<MatchLifecycleService> logger)
    {
        _store = store;
        _publisher = publisher;
        _logger = logger;
    }

    public Task<bool> IsValidTransitionAsync(LiveMatchStatus current, LiveMatchStatus target, CancellationToken cancellationToken = default)
    {
        var valid = ValidTransitions.TryGetValue(current, out var targets) && targets.Contains(target);
        return Task.FromResult(valid);
    }

    public Task<LiveMatch> TransitionToCheckInAsync(Guid matchId, CancellationToken cancellationToken = default) =>
        TransitionAsync(matchId, LiveMatchStatus.CheckInOpen, cancellationToken);

    public Task<LiveMatch> TransitionToWarmUpAsync(Guid matchId, CancellationToken cancellationToken = default) =>
        TransitionAsync(matchId, LiveMatchStatus.WarmUp, cancellationToken);

    public Task<LiveMatch> TransitionToLiveAsync(Guid matchId, CancellationToken cancellationToken = default) =>
        TransitionAsync(matchId, LiveMatchStatus.Live, cancellationToken);

    public Task<LiveMatch> TransitionToPausedAsync(Guid matchId, CancellationToken cancellationToken = default) =>
        TransitionAsync(matchId, LiveMatchStatus.Paused, cancellationToken);

    public Task<LiveMatch> TransitionToCancelledAsync(Guid matchId, CancellationToken cancellationToken = default) =>
        TransitionAsync(matchId, LiveMatchStatus.Cancelled, cancellationToken);

    public Task<LiveMatch> TransitionToAbandonedAsync(Guid matchId, CancellationToken cancellationToken = default) =>
        TransitionAsync(matchId, LiveMatchStatus.Abandoned, cancellationToken);

    public async Task<LiveMatch> TransitionToCompletedAsync(Guid matchId, Guid? winnerId, string? winnerName, CancellationToken cancellationToken = default)
    {
        var match = await TransitionAsync(matchId, LiveMatchStatus.Completed, cancellationToken);
        match.WinnerId = winnerId;
        match.WinnerName = winnerName;
        match.CompletedAt = DateTime.UtcNow;
        if (match.StartedAt.HasValue)
            match.TotalPlayTime = DateTime.UtcNow - match.StartedAt.Value;
        _store.Set(match);
        await _publisher.PublishScoreUpdateAsync(match, cancellationToken);
        return match;
    }

    public async Task<LiveMatch> RecordWalkoverAsync(Guid matchId, Guid winnerId, string winnerName, CancellationToken cancellationToken = default)
    {
        var match = await TransitionAsync(matchId, LiveMatchStatus.Walkover, cancellationToken);
        match.WinnerId = winnerId;
        match.WinnerName = winnerName;
        match.CompletedAt = DateTime.UtcNow;
        _store.Set(match);
        await _publisher.PublishScoreUpdateAsync(match, cancellationToken);
        return match;
    }

    public async Task<LiveMatch> RecordForfeitAsync(Guid matchId, Guid winnerId, string winnerName, CancellationToken cancellationToken = default)
    {
        var match = await TransitionAsync(matchId, LiveMatchStatus.Forfeit, cancellationToken);
        match.WinnerId = winnerId;
        match.WinnerName = winnerName;
        match.CompletedAt = DateTime.UtcNow;
        _store.Set(match);
        await _publisher.PublishScoreUpdateAsync(match, cancellationToken);
        return match;
    }

    private async Task<LiveMatch> TransitionAsync(Guid matchId, LiveMatchStatus targetStatus, CancellationToken cancellationToken)
    {
        var match = _store.Get(matchId) ?? throw new ArgumentException($"Live match not found: {matchId}");

        if (!await IsValidTransitionAsync(match.Status, targetStatus, cancellationToken))
            throw new InvalidOperationException($"Invalid transition from {match.Status} to {targetStatus}");

        var previousStatus = match.Status.ToString();
        match.Status = targetStatus;
        match.Version++;

        if (targetStatus == LiveMatchStatus.Live && match.StartedAt == null)
            match.StartedAt = DateTime.UtcNow;
        else if (targetStatus == LiveMatchStatus.Paused)
            match.PausedAt = DateTime.UtcNow;

        _store.Set(match);
        await _publisher.PublishMatchStatusChangeAsync(match, previousStatus, cancellationToken);

        _logger.LogInformation("Match {MatchId} transitioned from {From} to {To}", matchId, previousStatus, targetStatus);
        return match;
    }
}
