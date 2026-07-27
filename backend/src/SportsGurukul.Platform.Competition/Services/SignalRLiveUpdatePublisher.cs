using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.Competition.Interfaces;
using SportsGurukul.Platform.Competition.Models;

namespace SportsGurukul.Platform.Competition.Services;

public class SignalRLiveUpdatePublisher : ILiveUpdatePublisher
{
    private readonly ILogger<SignalRLiveUpdatePublisher> _logger;

    public SignalRLiveUpdatePublisher(ILogger<SignalRLiveUpdatePublisher> logger)
    {
        _logger = logger;
    }

    public Task PublishScoreUpdateAsync(LiveMatch match, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("SignalR publish: ScoreUpdate for match {MatchId}, Home: {Home}, Away: {Away}",
            match.Id, match.HomeScore.TotalPoints, match.AwayScore.TotalPoints);
        return Task.CompletedTask;
    }

    public Task PublishMatchStatusChangeAsync(LiveMatch match, string previousStatus, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("SignalR publish: StatusChange for match {MatchId}: {From} -> {To}",
            match.Id, previousStatus, match.Status);
        return Task.CompletedTask;
    }

    public Task PublishLeaderboardUpdateAsync(Leaderboard leaderboard, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("SignalR publish: LeaderboardUpdate for tournament {TournamentId}", leaderboard.TournamentId);
        return Task.CompletedTask;
    }

    public Task PublishRankingUpdateAsync(Guid tournamentId, IReadOnlyList<LeaderboardEntry> rankings, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("SignalR publish: RankingUpdate for tournament {TournamentId}", tournamentId);
        return Task.CompletedTask;
    }
}
