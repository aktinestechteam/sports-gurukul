using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Interfaces;

public interface ILiveScoringService
{
    Task<LiveMatch> StartMatchAsync(Guid tournamentId, Guid matchId, string sportCode, CancellationToken cancellationToken = default);
    Task<LiveMatch> UpdateScoreAsync(Guid matchId, Guid participantId, int points, ScoringUnit unit, int periodNumber, string? description, CancellationToken cancellationToken = default);
    Task<LiveMatch> UndoLastScoreAsync(Guid matchId, CancellationToken cancellationToken = default);
    Task<LiveMatch?> GetLiveMatchAsync(Guid matchId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LiveMatch>> GetLiveMatchesByTournamentAsync(Guid tournamentId, CancellationToken cancellationToken = default);
}
