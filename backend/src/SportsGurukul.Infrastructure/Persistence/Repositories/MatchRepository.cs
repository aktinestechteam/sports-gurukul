using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class MatchRepository : Repository<TournamentMatch>, IMatchRepository
{
    public MatchRepository(ApplicationDbContext context) : base(context) { }

    public async Task<TournamentMatch?> GetWithDetailsAsync(
        Guid matchId, CancellationToken cancellationToken = default)
    {
        return await Context.TournamentMatches
            .AsNoTracking()
            .Include(m => m.TournamentStage)
            .Include(m => m.TournamentRound)
            .Include(m => m.TournamentVenue)
            .Include(m => m.TournamentCourt)
            .Include(m => m.HomeParticipant)
            .Include(m => m.AwayParticipant)
            .Include(m => m.Winner)
            .Include(m => m.Sets.Where(s => !s.IsDeleted))
            .Include(m => m.Results.Where(r => !r.IsDeleted))
            .FirstOrDefaultAsync(m => m.Id == matchId && !m.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<TournamentMatch>> GetByTournamentIdAsync(
        Guid tournamentId, CancellationToken cancellationToken = default)
    {
        return await Context.TournamentMatches
            .AsNoTracking()
            .Where(m => m.TournamentId == tournamentId && !m.IsDeleted)
            .OrderBy(m => m.MatchNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TournamentMatch>> GetByStageIdAsync(
        Guid stageId, CancellationToken cancellationToken = default)
    {
        return await Context.TournamentMatches
            .AsNoTracking()
            .Where(m => m.TournamentStageId == stageId && !m.IsDeleted)
            .OrderBy(m => m.MatchNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TournamentMatch>> GetByRoundIdAsync(
        Guid roundId, CancellationToken cancellationToken = default)
    {
        return await Context.TournamentMatches
            .AsNoTracking()
            .Where(m => m.TournamentRoundId == roundId && !m.IsDeleted)
            .OrderBy(m => m.MatchNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TournamentMatch>> GetByStatusAsync(
        Guid tournamentId, MatchStatus status, CancellationToken cancellationToken = default)
    {
        return await Context.TournamentMatches
            .AsNoTracking()
            .Where(m => m.TournamentId == tournamentId && m.Status == status && !m.IsDeleted)
            .OrderBy(m => m.ScheduledDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TournamentMatch>> SearchAsync(
        Guid? tournamentId,
        MatchStatus? status,
        DateTime? dateFrom,
        DateTime? dateTo,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = Context.TournamentMatches
            .AsNoTracking()
            .Where(m => !m.IsDeleted);

        if (tournamentId.HasValue)
            query = query.Where(m => m.TournamentId == tournamentId.Value);

        if (status.HasValue)
            query = query.Where(m => m.Status == status.Value);

        if (dateFrom.HasValue)
            query = query.Where(m => m.ScheduledDate >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(m => m.ScheduledDate <= dateTo.Value);

        return await query
            .OrderBy(m => m.ScheduledDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountSearchAsync(
        Guid? tournamentId,
        MatchStatus? status,
        DateTime? dateFrom,
        DateTime? dateTo,
        CancellationToken cancellationToken = default)
    {
        var query = Context.TournamentMatches
            .AsNoTracking()
            .Where(m => !m.IsDeleted);

        if (tournamentId.HasValue)
            query = query.Where(m => m.TournamentId == tournamentId.Value);

        if (status.HasValue)
            query = query.Where(m => m.Status == status.Value);

        if (dateFrom.HasValue)
            query = query.Where(m => m.ScheduledDate >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(m => m.ScheduledDate <= dateTo.Value);

        return await query.CountAsync(cancellationToken);
    }
}
