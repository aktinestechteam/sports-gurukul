using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Interfaces;

public interface ILiveScoreCache
{
    Task<LiveMatch?> GetLiveMatchAsync(Guid matchId, CancellationToken cancellationToken = default);
    Task SetLiveMatchAsync(LiveMatch match, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task RemoveLiveMatchAsync(Guid matchId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LiveMatch>> GetTournamentLiveMatchesAsync(Guid tournamentId, CancellationToken cancellationToken = default);
    Task SetTournamentLiveMatchesAsync(Guid tournamentId, IReadOnlyList<LiveMatch> matches, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task<Leaderboard?> GetLeaderboardAsync(Guid tournamentId, LeaderboardType type, string? sportCode, CancellationToken cancellationToken = default);
    Task SetLeaderboardAsync(Guid tournamentId, LeaderboardType type, string? sportCode, Leaderboard leaderboard, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task<StandingsEntry[]?> GetStandingsAsync(Guid tournamentId, CancellationToken cancellationToken = default);
    Task SetStandingsAsync(Guid tournamentId, StandingsEntry[] standings, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task RemoveTournamentCacheAsync(Guid tournamentId, CancellationToken cancellationToken = default);
}
