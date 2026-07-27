using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class TournamentRepository : Repository<Tournament>, ITournamentRepository
{
    public TournamentRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Tournament?> GetWithDetailsAsync(
        Guid tournamentId, CancellationToken cancellationToken = default)
    {
        return await Context.Tournaments
            .AsNoTracking()
            .Include(t => t.Categories.Where(c => !c.IsDeleted))
            .Include(t => t.TournamentSports.Where(s => !s.IsDeleted))
            .Include(t => t.Venues.Where(v => !v.IsDeleted))
            .Include(t => t.Stages.Where(s => !s.IsDeleted))
            .Include(t => t.Registrations.Where(r => !r.IsDeleted))
            .Include(t => t.Participants.Where(p => !p.IsDeleted))
            .Include(t => t.Teams.Where(t2 => !t2.IsDeleted))
            .Include(t => t.Officials.Where(o => !o.IsDeleted))
            .Include(t => t.Sponsors.Where(s => !s.IsDeleted))
            .Include(t => t.Documents.Where(d => !d.IsDeleted))
            .Include(t => t.Gallery.Where(g => !g.IsDeleted))
            .Include(t => t.Rules_.Where(r => !r.IsDeleted))
            .Include(t => t.Rankings.Where(r => !r.IsDeleted))
            .Include(t => t.Awards.Where(a => !a.IsDeleted))
            .FirstOrDefaultAsync(t => t.Id == tournamentId && !t.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<Tournament>> GetByAcademyIdAsync(
        Guid academyId, CancellationToken cancellationToken = default)
    {
        return await Context.Tournaments
            .AsNoTracking()
            .Where(t => t.AcademyId == academyId && !t.IsDeleted)
            .OrderByDescending(t => t.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Tournament>> GetBySportIdAsync(
        Guid sportId, CancellationToken cancellationToken = default)
    {
        return await Context.Tournaments
            .AsNoTracking()
            .Where(t => t.SportId == sportId && !t.IsDeleted)
            .OrderByDescending(t => t.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Tournament>> SearchAsync(
        Guid? academyId,
        TournamentStatus? status,
        TournamentType? type,
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Tournaments
            .AsNoTracking()
            .Where(t => !t.IsDeleted);

        if (academyId.HasValue)
            query = query.Where(t => t.AcademyId == academyId.Value);

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        if (type.HasValue)
            query = query.Where(t => t.TournamentType == type.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(t =>
                t.TournamentName.Contains(searchTerm) ||
                t.TournamentCode.Contains(searchTerm) ||
                (t.Description != null && t.Description.Contains(searchTerm)));

        return await query
            .OrderByDescending(t => t.StartDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountSearchAsync(
        Guid? academyId,
        TournamentStatus? status,
        TournamentType? type,
        string? searchTerm,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Tournaments
            .AsNoTracking()
            .Where(t => !t.IsDeleted);

        if (academyId.HasValue)
            query = query.Where(t => t.AcademyId == academyId.Value);

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        if (type.HasValue)
            query = query.Where(t => t.TournamentType == type.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(t =>
                t.TournamentName.Contains(searchTerm) ||
                t.TournamentCode.Contains(searchTerm) ||
                (t.Description != null && t.Description.Contains(searchTerm)));

        return await query.CountAsync(cancellationToken);
    }

    public async Task<bool> IsTournamentCodeUniqueAsync(
        string tournamentCode, CancellationToken cancellationToken = default)
    {
        return await Context.Tournaments
            .AsNoTracking()
            .AnyAsync(t => t.TournamentCode == tournamentCode && !t.IsDeleted, cancellationToken);
    }
}
