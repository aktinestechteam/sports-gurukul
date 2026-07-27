using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IRankingRepository : IRepository<TournamentRanking>
{
    Task<TournamentRanking?> GetByParticipantAsync(Guid tournamentId, Guid participantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TournamentRanking>> GetByTournamentIdAsync(Guid tournamentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TournamentRanking>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TournamentRanking>> GetTopRankingsAsync(Guid tournamentId, int count, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TournamentRanking>> GetTopRankingsByCategoryAsync(Guid categoryId, int count, CancellationToken cancellationToken = default);
}
