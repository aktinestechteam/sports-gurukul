using SportsGurukul.Platform.Competition.Models;

namespace SportsGurukul.Platform.Competition.Interfaces;

public interface IStatisticsService
{
    Task<MatchStatistics> GetMatchStatisticsAsync(Guid matchId, CancellationToken cancellationToken = default);
    Task<PlayerStatistics> GetPlayerStatisticsAsync(Guid participantId, string? sportCode, CancellationToken cancellationToken = default);
    Task<TeamStatistics> GetTeamStatisticsAsync(Guid teamId, string? sportCode, CancellationToken cancellationToken = default);
    Task GenerateMatchStatisticsAsync(Guid matchId, CancellationToken cancellationToken = default);
    Task GeneratePlayerStatisticsAsync(Guid participantId, string? sportCode, CancellationToken cancellationToken = default);
    Task GenerateTeamStatisticsAsync(Guid teamId, string? sportCode, CancellationToken cancellationToken = default);
}
