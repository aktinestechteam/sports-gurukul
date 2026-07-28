using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IEventAttendanceRepository : IRepository<EventAttendance>
{
    Task<IReadOnlyList<EventAttendance>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EventAttendance>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EventAttendance>> GetByParticipantIdAsync(Guid participantId, CancellationToken cancellationToken = default);
    Task<EventAttendance?> GetBySessionAndParticipantAsync(Guid sessionId, Guid participantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EventAttendance>> GetByStatusAsync(Guid eventId, EventAttendanceStatus status, CancellationToken cancellationToken = default);
    Task<int> GetAttendeeCountAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<int> GetSessionAttendeeCountAsync(Guid sessionId, CancellationToken cancellationToken = default);
}
