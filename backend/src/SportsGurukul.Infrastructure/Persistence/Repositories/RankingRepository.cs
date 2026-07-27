using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class RankingRepository : Repository<TournamentRanking>, IRankingRepository
{
    public RankingRepository(ApplicationDbContext context) : base(context) { }

    public async Task<TournamentRanking?> GetByParticipantAsync(
        Guid tournamentId, Guid participantId, CancellationToken cancellationToken = default)
    {
        return await Context.TournamentRankings
            .AsNoTracking()
            .FirstOrDefaultAsync(r =>
                r.TournamentId == tournamentId &&
                r.ParticipantId == participantId &&
                !r.IsDeleted,
                cancellationToken);
    }

    public async Task<IReadOnlyList<TournamentRanking>> GetByTournamentIdAsync(
        Guid tournamentId, CancellationToken cancellationToken = default)
    {
        return await Context.TournamentRankings
            .AsNoTracking()
            .Where(r => r.TournamentId == tournamentId && !r.IsDeleted)
            .OrderBy(r => r.Rank)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TournamentRanking>> GetByCategoryIdAsync(
        Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await Context.TournamentRankings
            .AsNoTracking()
            .Where(r => r.CategoryId == categoryId && !r.IsDeleted)
            .OrderBy(r => r.Rank)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TournamentRanking>> GetTopRankingsAsync(
        Guid tournamentId, int count, CancellationToken cancellationToken = default)
    {
        return await Context.TournamentRankings
            .AsNoTracking()
            .Where(r => r.TournamentId == tournamentId && !r.IsDeleted)
            .OrderBy(r => r.Rank)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TournamentRanking>> GetTopRankingsByCategoryAsync(
        Guid categoryId, int count, CancellationToken cancellationToken = default)
    {
        return await Context.TournamentRankings
            .AsNoTracking()
            .Where(r => r.CategoryId == categoryId && !r.IsDeleted)
            .OrderBy(r => r.Rank)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}
