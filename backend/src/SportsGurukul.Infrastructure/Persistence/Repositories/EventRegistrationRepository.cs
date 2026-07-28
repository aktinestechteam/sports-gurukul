using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class EventRegistrationRepository : Repository<EventRegistration>, IEventRegistrationRepository
{
    public EventRegistrationRepository(ApplicationDbContext context) : base(context) { }

    public async Task<EventRegistration?> GetByRegistrationNumberAsync(string registrationNumber, CancellationToken cancellationToken = default)
    {
        return await Context.EventRegistrations
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.RegistrationNumber == registrationNumber && !r.IsDeleted, cancellationToken);
    }

    public async Task<EventRegistration?> GetWithDetailsAsync(Guid registrationId, CancellationToken cancellationToken = default)
    {
        return await Context.EventRegistrations
            .AsNoTracking()
            .Include(r => r.Event)
            .Include(r => r.Athlete)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == registrationId && !r.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<EventRegistration>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return await Context.EventRegistrations
            .AsNoTracking()
            .Where(r => r.EventId == eventId && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EventRegistration>> GetByEventIdWithStatusAsync(Guid eventId, EventRegistrationStatus status, CancellationToken cancellationToken = default)
    {
        return await Context.EventRegistrations
            .AsNoTracking()
            .Where(r => r.EventId == eventId && r.Status == status && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EventRegistration>> GetByAthleteIdAsync(Guid athleteId, CancellationToken cancellationToken = default)
    {
        return await Context.EventRegistrations
            .AsNoTracking()
            .Where(r => r.AthleteId == athleteId && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EventRegistration>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.EventRegistrations
            .AsNoTracking()
            .Where(r => r.UserId == userId && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsAlreadyRegisteredAsync(Guid eventId, Guid? athleteId, Guid? userId, CancellationToken cancellationToken = default)
    {
        if (athleteId.HasValue)
        {
            return await Context.EventRegistrations
                .AsNoTracking()
                .AnyAsync(r => r.EventId == eventId && r.AthleteId == athleteId.Value && !r.IsDeleted, cancellationToken);
        }

        if (userId.HasValue)
        {
            return await Context.EventRegistrations
                .AsNoTracking()
                .AnyAsync(r => r.EventId == eventId && r.UserId == userId.Value && !r.IsDeleted, cancellationToken);
        }

        return false;
    }

    public async Task<int> GetRegistrationCountAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return await Context.EventRegistrations
            .AsNoTracking()
            .CountAsync(r => r.EventId == eventId && !r.IsDeleted &&
                r.Status != EventRegistrationStatus.Cancelled &&
                r.Status != EventRegistrationStatus.Rejected, cancellationToken);
    }

    public async Task<IReadOnlyList<EventRegistration>> SearchAsync(
        Guid? eventId,
        EventRegistrationStatus? status,
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = Context.EventRegistrations
            .AsNoTracking()
            .Where(r => !r.IsDeleted);

        if (eventId.HasValue)
            query = query.Where(r => r.EventId == eventId.Value);

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(r =>
                r.RegistrationNumber.Contains(searchTerm) ||
                (r.Notes != null && r.Notes.Contains(searchTerm)));

        return await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountSearchAsync(
        Guid? eventId,
        EventRegistrationStatus? status,
        string? searchTerm,
        CancellationToken cancellationToken = default)
    {
        var query = Context.EventRegistrations
            .AsNoTracking()
            .Where(r => !r.IsDeleted);

        if (eventId.HasValue)
            query = query.Where(r => r.EventId == eventId.Value);

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(r =>
                r.RegistrationNumber.Contains(searchTerm) ||
                (r.Notes != null && r.Notes.Contains(searchTerm)));

        return await query.CountAsync(cancellationToken);
    }

    public async Task<bool> IsRegistrationNumberUniqueAsync(string registrationNumber, CancellationToken cancellationToken = default)
    {
        return !await Context.EventRegistrations
            .AsNoTracking()
            .AnyAsync(r => r.RegistrationNumber == registrationNumber && !r.IsDeleted, cancellationToken);
    }
}
