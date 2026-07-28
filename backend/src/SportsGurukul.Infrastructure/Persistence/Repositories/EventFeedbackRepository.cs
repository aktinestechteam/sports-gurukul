using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class EventFeedbackRepository : Repository<EventFeedback>, IEventFeedbackRepository
{
    public EventFeedbackRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<EventFeedback>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return await Context.EventFeedbacks
            .AsNoTracking()
            .Where(f => f.EventId == eventId && !f.IsDeleted)
            .Include(f => f.Participant)
            .Include(f => f.User)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EventFeedback>> GetByParticipantIdAsync(Guid participantId, CancellationToken cancellationToken = default)
    {
        return await Context.EventFeedbacks
            .AsNoTracking()
            .Where(f => f.ParticipantId == participantId && !f.IsDeleted)
            .Include(f => f.Event)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<EventFeedback?> GetByEventAndUserAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.EventFeedbacks
            .AsNoTracking()
            .FirstOrDefaultAsync(f =>
                f.EventId == eventId &&
                f.UserId == userId &&
                !f.IsDeleted, cancellationToken);
    }

    public async Task<double> GetAverageRatingAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var ratings = await Context.EventFeedbacks
            .AsNoTracking()
            .Where(f => f.EventId == eventId && !f.IsDeleted)
            .Select(f => (int)f.OverallRating)
            .ToListAsync(cancellationToken);

        return ratings.Count > 0 ? ratings.Average() : 0;
    }

    public async Task<int> GetFeedbackCountAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return await Context.EventFeedbacks
            .AsNoTracking()
            .CountAsync(f => f.EventId == eventId && !f.IsDeleted, cancellationToken);
    }
}
