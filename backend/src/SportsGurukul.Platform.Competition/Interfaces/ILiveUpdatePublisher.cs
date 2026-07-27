using SportsGurukul.Platform.Competition.Models;

namespace SportsGurukul.Platform.Competition.Interfaces;

public interface ILiveUpdatePublisher
{
    Task PublishScoreUpdateAsync(LiveMatch match, CancellationToken cancellationToken = default);
    Task PublishMatchStatusChangeAsync(LiveMatch match, string previousStatus, CancellationToken cancellationToken = default);
    Task PublishLeaderboardUpdateAsync(Leaderboard leaderboard, CancellationToken cancellationToken = default);
    Task PublishRankingUpdateAsync(Guid tournamentId, IReadOnlyList<LeaderboardEntry> rankings, CancellationToken cancellationToken = default);
}
