using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.EventManagement.Services;

public interface IEventAnnouncementService
{
    Task<bool> CanPublishAnnouncementAsync(Event evt, CancellationToken cancellationToken = default);
    Task<int> GetPublishedAnnouncementCountAsync(Guid eventId, CancellationToken cancellationToken = default);
}
