using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class BracketRepository : Repository<TournamentBracket>, IBracketRepository
{
    public BracketRepository(ApplicationDbContext context) : base(context) { }

    public async Task<TournamentBracket?> GetWithDetailsAsync(
        Guid bracketId, CancellationToken cancellationToken = default)
    {
        return await Context.TournamentBrackets
            .AsNoTracking()
            .Include(b => b.Tournament)
            .Include(b => b.Division)
            .FirstOrDefaultAsync(b => b.Id == bracketId && !b.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<TournamentBracket>> GetByTournamentIdAsync(
        Guid tournamentId, CancellationToken cancellationToken = default)
    {
        return await Context.TournamentBrackets
            .AsNoTracking()
            .Where(b => b.TournamentId == tournamentId && !b.IsDeleted)
            .OrderBy(b => b.BracketName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TournamentBracket>> GetByDivisionIdAsync(
        Guid divisionId, CancellationToken cancellationToken = default)
    {
        return await Context.TournamentBrackets
            .AsNoTracking()
            .Where(b => b.DivisionId == divisionId && !b.IsDeleted)
            .OrderBy(b => b.BracketName)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasBracketForDivisionAsync(
        Guid tournamentId, Guid divisionId, CancellationToken cancellationToken = default)
    {
        return await Context.TournamentBrackets
            .AsNoTracking()
            .AnyAsync(b =>
                b.TournamentId == tournamentId &&
                b.DivisionId == divisionId &&
                !b.IsDeleted,
                cancellationToken);
    }
}
