using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IEventFeedbackRepository : IRepository<EventFeedback>
{
    Task<IReadOnlyList<EventFeedback>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EventFeedback>> GetByParticipantIdAsync(Guid participantId, CancellationToken cancellationToken = default);
    Task<EventFeedback?> GetByEventAndUserAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default);
    Task<double> GetAverageRatingAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<int> GetFeedbackCountAsync(Guid eventId, CancellationToken cancellationToken = default);
}
