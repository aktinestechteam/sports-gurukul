using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class EventAttendanceRepository : Repository<EventAttendance>, IEventAttendanceRepository
{
    public EventAttendanceRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<EventAttendance>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return await Context.EventAttendances
            .AsNoTracking()
            .Where(a => a.EventId == eventId && !a.IsDeleted)
            .Include(a => a.Participant)
            .OrderBy(a => a.Participant.ParticipantName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EventAttendance>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await Context.EventAttendances
            .AsNoTracking()
            .Where(a => a.SessionId == sessionId && !a.IsDeleted)
            .Include(a => a.Participant)
            .OrderBy(a => a.Participant.ParticipantName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EventAttendance>> GetByParticipantIdAsync(Guid participantId, CancellationToken cancellationToken = default)
    {
        return await Context.EventAttendances
            .AsNoTracking()
            .Where(a => a.ParticipantId == participantId && !a.IsDeleted)
            .Include(a => a.Event)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<EventAttendance?> GetBySessionAndParticipantAsync(Guid sessionId, Guid participantId, CancellationToken cancellationToken = default)
    {
        return await Context.EventAttendances
            .AsNoTracking()
            .FirstOrDefaultAsync(a =>
                a.SessionId == sessionId &&
                a.ParticipantId == participantId &&
                !a.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<EventAttendance>> GetByStatusAsync(Guid eventId, EventAttendanceStatus status, CancellationToken cancellationToken = default)
    {
        return await Context.EventAttendances
            .AsNoTracking()
            .Where(a => a.EventId == eventId && a.Status == status && !a.IsDeleted)
            .Include(a => a.Participant)
            .OrderBy(a => a.Participant.ParticipantName)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetAttendeeCountAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return await Context.EventAttendances
            .AsNoTracking()
            .CountAsync(a => a.EventId == eventId && !a.IsDeleted &&
                (a.Status == EventAttendanceStatus.Present ||
                 a.Status == EventAttendanceStatus.CheckedIn ||
                 a.Status == EventAttendanceStatus.Late), cancellationToken);
    }

    public async Task<int> GetSessionAttendeeCountAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await Context.EventAttendances
            .AsNoTracking()
            .CountAsync(a => a.SessionId == sessionId && !a.IsDeleted &&
                (a.Status == EventAttendanceStatus.Present ||
                 a.Status == EventAttendanceStatus.CheckedIn ||
                 a.Status == EventAttendanceStatus.Late), cancellationToken);
    }
}
