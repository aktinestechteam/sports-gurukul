using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class EventRepository : Repository<Event>, IEventRepository
{
    public EventRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Event?> GetByEventCodeAsync(string eventCode, CancellationToken cancellationToken = default)
    {
        return await Context.Events
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EventCode == eventCode && !e.IsDeleted, cancellationToken);
    }

    public async Task<Event?> GetWithDetailsAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return await Context.Events
            .AsNoTracking()
            .Include(e => e.EventType)
            .Include(e => e.EventCategory)
            .Include(e => e.Sport)
            .Include(e => e.Schedules.Where(s => !s.IsDeleted))
            .Include(e => e.Venues.Where(v => !v.IsDeleted))
            .Include(e => e.Registrations.Where(r => !r.IsDeleted))
            .Include(e => e.Participants.Where(p => !p.IsDeleted))
            .Include(e => e.Speakers.Where(s => !s.IsDeleted))
            .Include(e => e.Coaches.Where(c => !c.IsDeleted))
            .Include(e => e.Volunteers.Where(v => !v.IsDeleted))
            .Include(e => e.Sponsors.Where(s => !s.IsDeleted))
            .Include(e => e.Sessions.Where(s => !s.IsDeleted))
            .Include(e => e.Agendas.Where(a => !a.IsDeleted))
            .Include(e => e.Tickets.Where(t => !t.IsDeleted))
            .Include(e => e.Attendances.Where(a => !a.IsDeleted))
            .Include(e => e.Certificates.Where(c => !c.IsDeleted))
            .Include(e => e.Feedbacks.Where(f => !f.IsDeleted))
            .Include(e => e.Media.Where(m => !m.IsDeleted))
            .Include(e => e.Documents.Where(d => !d.IsDeleted))
            .Include(e => e.Announcements.Where(a => !a.IsDeleted))
            .FirstOrDefaultAsync(e => e.Id == eventId && !e.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<Event>> GetByAcademyIdAsync(Guid academyId, CancellationToken cancellationToken = default)
    {
        return await Context.Events
            .AsNoTracking()
            .Where(e => e.AcademyId == academyId && !e.IsDeleted)
            .OrderByDescending(e => e.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Event>> GetBySportIdAsync(Guid sportId, CancellationToken cancellationToken = default)
    {
        return await Context.Events
            .AsNoTracking()
            .Where(e => e.SportId == sportId && !e.IsDeleted)
            .OrderByDescending(e => e.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Event>> GetByTypeIdAsync(Guid typeId, CancellationToken cancellationToken = default)
    {
        return await Context.Events
            .AsNoTracking()
            .Where(e => e.EventTypeId == typeId && !e.IsDeleted)
            .OrderByDescending(e => e.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Event>> GetByStatusAsync(EventStatus status, CancellationToken cancellationToken = default)
    {
        return await Context.Events
            .AsNoTracking()
            .Where(e => e.Status == status && !e.IsDeleted)
            .OrderByDescending(e => e.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Event>> GetUpcomingEventsAsync(Guid? academyId, int limit, CancellationToken cancellationToken = default)
    {
        var query = Context.Events
            .AsNoTracking()
            .Where(e => !e.IsDeleted && e.StartDate >= DateTime.UtcNow && e.Status != EventStatus.Draft);

        if (academyId.HasValue)
            query = query.Where(e => e.AcademyId == academyId.Value);

        return await query
            .OrderBy(e => e.StartDate)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Event>> SearchAsync(
        Guid? academyId,
        Guid? sportId,
        EventStatus? status,
        EventType? eventType,
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Events
            .AsNoTracking()
            .Where(e => !e.IsDeleted);

        if (academyId.HasValue)
            query = query.Where(e => e.AcademyId == academyId.Value);

        if (sportId.HasValue)
            query = query.Where(e => e.SportId == sportId.Value);

        if (status.HasValue)
            query = query.Where(e => e.Status == status.Value);

        if (eventType.HasValue)
        {
            var typeId = await Context.EventTypes
                .AsNoTracking()
                .Where(t => !t.IsDeleted)
                .Select(t => new { t.Id, t.Code })
                .ToListAsync(cancellationToken);

            var matchingCode = eventType.Value switch
            {
                EventType.Camp => "CAMP",
                EventType.Workshop => "WORKSHOP",
                EventType.Seminar => "SEMINAR",
                EventType.CoachingClinic => "COACHING_CLINIC",
                EventType.Trial => "TRIAL",
                EventType.TalentHunt => "TALENT_HUNT",
                EventType.Competition => "COMPETITION",
                EventType.CommunityEvent => "COMMUNITY_EVENT",
                EventType.SportsFestival => "SPORTS_FESTIVAL",
                EventType.Webinar => "WEBINAR",
                _ => null
            };

            if (matchingCode != null)
            {
                var matchingType = typeId.FirstOrDefault(t => t.Code == matchingCode);
                if (matchingType != null)
                    query = query.Where(e => e.EventTypeId == matchingType.Id);
            }
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(e =>
                e.EventName.Contains(searchTerm) ||
                e.EventCode.Contains(searchTerm) ||
                (e.Description != null && e.Description.Contains(searchTerm)));

        return await query
            .OrderByDescending(e => e.StartDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountSearchAsync(
        Guid? academyId,
        Guid? sportId,
        EventStatus? status,
        EventType? eventType,
        string? searchTerm,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Events
            .AsNoTracking()
            .Where(e => !e.IsDeleted);

        if (academyId.HasValue)
            query = query.Where(e => e.AcademyId == academyId.Value);

        if (sportId.HasValue)
            query = query.Where(e => e.SportId == sportId.Value);

        if (status.HasValue)
            query = query.Where(e => e.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(e =>
                e.EventName.Contains(searchTerm) ||
                e.EventCode.Contains(searchTerm) ||
                (e.Description != null && e.Description.Contains(searchTerm)));

        return await query.CountAsync(cancellationToken);
    }

    public async Task<bool> IsEventCodeUniqueAsync(string eventCode, CancellationToken cancellationToken = default)
    {
        return !await Context.Events
            .AsNoTracking()
            .AnyAsync(e => e.EventCode == eventCode && !e.IsDeleted, cancellationToken);
    }
}
