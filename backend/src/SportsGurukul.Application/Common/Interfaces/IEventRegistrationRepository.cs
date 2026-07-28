using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IEventRegistrationRepository : IRepository<EventRegistration>
{
    Task<EventRegistration?> GetByRegistrationNumberAsync(string registrationNumber, CancellationToken cancellationToken = default);
    Task<EventRegistration?> GetWithDetailsAsync(Guid registrationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EventRegistration>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EventRegistration>> GetByEventIdWithStatusAsync(Guid eventId, EventRegistrationStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EventRegistration>> GetByAthleteIdAsync(Guid athleteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EventRegistration>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> IsAlreadyRegisteredAsync(Guid eventId, Guid? athleteId, Guid? userId, CancellationToken cancellationToken = default);
    Task<int> GetRegistrationCountAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EventRegistration>> SearchAsync(
        Guid? eventId,
        EventRegistrationStatus? status,
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<int> CountSearchAsync(
        Guid? eventId,
        EventRegistrationStatus? status,
        string? searchTerm,
        CancellationToken cancellationToken = default);
    Task<bool> IsRegistrationNumberUniqueAsync(string registrationNumber, CancellationToken cancellationToken = default);
}
