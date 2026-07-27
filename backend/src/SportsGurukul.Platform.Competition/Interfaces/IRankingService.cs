using SportsGurukul.Platform.Competition.Models;

namespace SportsGurukul.Platform.Competition.Interfaces;

public interface IRankingService
{
    Task<IReadOnlyList<LeaderboardEntry>> CalculateRankingsAsync(Guid tournamentId, string? sportCode, CancellationToken cancellationToken = default);
    Task<LeaderboardEntry?> GetParticipantRankingAsync(Guid tournamentId, Guid participantId, CancellationToken cancellationToken = default);
    Task UpdateRankingsAfterMatchAsync(Guid tournamentId, Guid homeParticipantId, Guid awayParticipantId, int homeScore, int awayScore, CancellationToken cancellationToken = default);
}
