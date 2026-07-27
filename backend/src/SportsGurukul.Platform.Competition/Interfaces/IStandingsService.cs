using SportsGurukul.Platform.Competition.Models;

namespace SportsGurukul.Platform.Competition.Interfaces;

public interface IStandingsService
{
    Task<IReadOnlyList<StandingsEntry>> GetTournamentStandingsAsync(Guid tournamentId, string? sportCode, CancellationToken cancellationToken = default);
    Task<StandingsEntry?> GetParticipantStandingAsync(Guid tournamentId, Guid participantId, CancellationToken cancellationToken = default);
    Task UpdateStandingsAfterMatchAsync(Guid tournamentId, Guid homeParticipantId, Guid awayParticipantId, int homeScore, int awayScore, CancellationToken cancellationToken = default);
}
