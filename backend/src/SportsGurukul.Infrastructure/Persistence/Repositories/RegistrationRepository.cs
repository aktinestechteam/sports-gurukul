using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class RegistrationRepository : Repository<TournamentRegistration>, IRegistrationRepository
{
    public RegistrationRepository(ApplicationDbContext context) : base(context) { }

    public async Task<TournamentRegistration?> GetWithDetailsAsync(
        Guid registrationId, CancellationToken cancellationToken = default)
    {
        return await Context.TournamentRegistrations
            .AsNoTracking()
            .Include(r => r.Tournament)
            .Include(r => r.Category)
            .Include(r => r.Division)
            .Include(r => r.Athlete)
            .Include(r => r.Team)
            .Include(r => r.Academy)
            .FirstOrDefaultAsync(r => r.Id == registrationId && !r.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<TournamentRegistration>> GetByTournamentIdAsync(
        Guid tournamentId, CancellationToken cancellationToken = default)
    {
        return await Context.TournamentRegistrations
            .AsNoTracking()
            .Where(r => r.TournamentId == tournamentId && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TournamentRegistration>> GetByCategoryIdAsync(
        Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await Context.TournamentRegistrations
            .AsNoTracking()
            .Where(r => r.CategoryId == categoryId && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TournamentRegistration>> GetByStatusAsync(
        Guid tournamentId, TournamentRegistrationStatus status, CancellationToken cancellationToken = default)
    {
        return await Context.TournamentRegistrations
            .AsNoTracking()
            .Where(r => r.TournamentId == tournamentId && r.RegistrationStatus == status && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsAlreadyRegisteredAsync(
        Guid tournamentId, Guid? athleteId, Guid? teamId, CancellationToken cancellationToken = default)
    {
        return await Context.TournamentRegistrations
            .AsNoTracking()
            .AnyAsync(r =>
                r.TournamentId == tournamentId &&
                !r.IsDeleted &&
                ((athleteId.HasValue && r.AthleteId == athleteId.Value) ||
                 (teamId.HasValue && r.TeamId == teamId.Value)),
                cancellationToken);
    }

    public async Task<int> GetRegistrationCountAsync(
        Guid tournamentId, CancellationToken cancellationToken = default)
    {
        return await Context.TournamentRegistrations
            .AsNoTracking()
            .CountAsync(r => r.TournamentId == tournamentId && !r.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<TournamentRegistration>> SearchAsync(
        Guid? tournamentId,
        TournamentRegistrationStatus? status,
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = Context.TournamentRegistrations
            .AsNoTracking()
            .Where(r => !r.IsDeleted);

        if (tournamentId.HasValue)
            query = query.Where(r => r.TournamentId == tournamentId.Value);

        if (status.HasValue)
            query = query.Where(r => r.RegistrationStatus == status.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(r =>
                r.RegistrantName.Contains(searchTerm) ||
                (r.Email != null && r.Email.Contains(searchTerm)));

        return await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountSearchAsync(
        Guid? tournamentId,
        TournamentRegistrationStatus? status,
        string? searchTerm,
        CancellationToken cancellationToken = default)
    {
        var query = Context.TournamentRegistrations
            .AsNoTracking()
            .Where(r => !r.IsDeleted);

        if (tournamentId.HasValue)
            query = query.Where(r => r.TournamentId == tournamentId.Value);

        if (status.HasValue)
            query = query.Where(r => r.RegistrationStatus == status.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(r =>
                r.RegistrantName.Contains(searchTerm) ||
                (r.Email != null && r.Email.Contains(searchTerm)));

        return await query.CountAsync(cancellationToken);
    }
}
