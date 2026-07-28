using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Services;

public class EventLifecycleService : IEventLifecycleService
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<EventLifecycleService> _logger;

    public EventLifecycleService(IEventRepository eventRepository, ILogger<EventLifecycleService> logger)
    {
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task<string> GenerateEventCodeAsync(CancellationToken cancellationToken = default)
    {
        var date = DateTime.UtcNow;
        var sequence = await _eventRepository.CountSearchAsync(null, null, null, null, null, cancellationToken) + 1;
        var code = $"EVT-{date:yyyyMMdd}-{sequence:D4}";
        _logger.LogInformation("Generated event code: {EventCode}", code);
        return code;
    }

    public Task<EventStatus> ValidateStateTransitionAsync(EventStatus current, EventStatus target, CancellationToken cancellationToken = default)
    {
        var validTransitions = current switch
        {
            EventStatus.Draft => new[] { EventStatus.Published, EventStatus.Cancelled, EventStatus.Archived },
            EventStatus.Published => new[] { EventStatus.RegistrationOpen, EventStatus.Scheduled, EventStatus.Cancelled, EventStatus.Archived },
            EventStatus.RegistrationOpen => new[] { EventStatus.RegistrationClosed, EventStatus.Scheduled, EventStatus.Cancelled },
            EventStatus.RegistrationClosed => new[] { EventStatus.Scheduled, EventStatus.Cancelled },
            EventStatus.Scheduled => new[] { EventStatus.InProgress, EventStatus.Cancelled },
            EventStatus.InProgress => new[] { EventStatus.Completed, EventStatus.Cancelled },
            EventStatus.Completed => new[] { EventStatus.Archived },
            EventStatus.Cancelled => new[] { EventStatus.Archived },
            EventStatus.Archived => Array.Empty<EventStatus>(),
            _ => Array.Empty<EventStatus>()
        };

        if (validTransitions.Contains(target))
            return Task.FromResult(target);

        _logger.LogWarning("Invalid state transition from {Current} to {Target}", current, target);
        throw new InvalidOperationException($"Cannot transition from {current} to {target}.");
    }

    public Task<bool> CanPublishAsync(Event evt, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(evt.Status == EventStatus.Draft &&
            !string.IsNullOrEmpty(evt.EventName) &&
            evt.StartDate > DateTime.UtcNow &&
            evt.EndDate > evt.StartDate);
    }

    public Task<bool> CanStartAsync(Event evt, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(evt.Status == EventStatus.Scheduled &&
            DateTime.UtcNow >= evt.StartDate.AddDays(-1));
    }

    public Task<bool> CanCompleteAsync(Event evt, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(evt.Status == EventStatus.InProgress &&
            DateTime.UtcNow >= evt.EndDate);
    }

    public Task<bool> CanArchiveAsync(Event evt, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(evt.Status is EventStatus.Completed or EventStatus.Cancelled);
    }

    public Task<bool> CanCancelAsync(Event evt, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(evt.Status is not (EventStatus.Archived or EventStatus.Completed or EventStatus.InProgress));
    }
}
