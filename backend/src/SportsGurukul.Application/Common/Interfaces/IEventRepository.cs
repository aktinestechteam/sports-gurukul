using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IEventRepository : IRepository<Event>
{
    Task<Event?> GetByEventCodeAsync(string eventCode, CancellationToken cancellationToken = default);
    Task<Event?> GetWithDetailsAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Event>> GetByAcademyIdAsync(Guid academyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Event>> GetBySportIdAsync(Guid sportId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Event>> GetByTypeIdAsync(Guid typeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Event>> GetByStatusAsync(EventStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Event>> GetUpcomingEventsAsync(Guid? academyId, int limit, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Event>> SearchAsync(
        Guid? academyId,
        Guid? sportId,
        EventStatus? status,
        EventType? eventType,
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<int> CountSearchAsync(
        Guid? academyId,
        Guid? sportId,
        EventStatus? status,
        EventType? eventType,
        string? searchTerm,
        CancellationToken cancellationToken = default);
    Task<bool> IsEventCodeUniqueAsync(string eventCode, CancellationToken cancellationToken = default);
}
