using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Services;

public interface IEventRegistrationService
{
    Task<string> GenerateRegistrationNumberAsync(CancellationToken cancellationToken = default);
    Task<bool> IsRegistrationAllowedAsync(Event evt, CancellationToken cancellationToken = default);
    Task<bool> IsCapacityAvailableAsync(Event evt, CancellationToken cancellationToken = default);
    Task<bool> IsDuplicateRegistrationAsync(Guid eventId, Guid? athleteId, Guid? userId, CancellationToken cancellationToken = default);
    Task<int> GetCurrentRegistrationCountAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<EventRegistration?> ProcessWaitlistPromotionAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<EventRegistrationStatus> DetermineInitialStatusAsync(Event evt, CancellationToken cancellationToken = default);
}
