using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Services;

public class EventAnnouncementService : IEventAnnouncementService
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<EventAnnouncementService> _logger;

    public EventAnnouncementService(IEventRepository eventRepository, ILogger<EventAnnouncementService> logger)
    {
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public Task<bool> CanPublishAnnouncementAsync(Event evt, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(evt.Status is not (EventStatus.Draft or EventStatus.Archived));
    }

    public Task<int> GetPublishedAnnouncementCountAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var evt = _eventRepository.GetByIdAsync(eventId, cancellationToken).Result;
        return Task.FromResult(evt?.Announcements?.Count(a => a.IsPublished && !a.IsDeleted) ?? 0);
    }
}
