using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.EventManagement.Services;

public interface IEventCertificateService
{
    Task<string> GenerateCertificateNumberAsync(CancellationToken cancellationToken = default);
    Task<bool> IsEligibleForCertificateAsync(EventParticipant participant, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EventParticipant>> GetEligibleParticipantsAsync(Guid eventId, CancellationToken cancellationToken = default);
}
