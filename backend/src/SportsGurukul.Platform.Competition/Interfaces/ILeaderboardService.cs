using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Interfaces;

public interface ILeaderboardService
{
    Task<Leaderboard> GenerateLeaderboardAsync(Guid tournamentId, LeaderboardType type, string? sportCode, CancellationToken cancellationToken = default);
    Task<Leaderboard?> GetLeaderboardAsync(Guid tournamentId, LeaderboardType type, string? sportCode, CancellationToken cancellationToken = default);
    Task UpdateLeaderboardAfterMatchAsync(Guid tournamentId, LeaderboardType type, string? sportCode, CancellationToken cancellationToken = default);
}
