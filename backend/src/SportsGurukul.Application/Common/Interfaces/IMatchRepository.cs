using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IMatchRepository : IRepository<TournamentMatch>
{
    Task<TournamentMatch?> GetWithDetailsAsync(Guid matchId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TournamentMatch>> GetByTournamentIdAsync(Guid tournamentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TournamentMatch>> GetByStageIdAsync(Guid stageId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TournamentMatch>> GetByRoundIdAsync(Guid roundId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TournamentMatch>> GetByStatusAsync(Guid tournamentId, MatchStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TournamentMatch>> SearchAsync(
        Guid? tournamentId,
        MatchStatus? status,
        DateTime? dateFrom,
        DateTime? dateTo,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<int> CountSearchAsync(
        Guid? tournamentId,
        MatchStatus? status,
        DateTime? dateFrom,
        DateTime? dateTo,
        CancellationToken cancellationToken = default);
}
