using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Services;

public class EventRegistrationService : IEventRegistrationService
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventRegistrationRepository _registrationRepository;
    private readonly ILogger<EventRegistrationService> _logger;

    public EventRegistrationService(
        IEventRepository eventRepository,
        IEventRegistrationRepository registrationRepository,
        ILogger<EventRegistrationService> logger)
    {
        _eventRepository = eventRepository;
        _registrationRepository = registrationRepository;
        _logger = logger;
    }

    public async Task<string> GenerateRegistrationNumberAsync(CancellationToken cancellationToken = default)
    {
        var count = await _registrationRepository.CountSearchAsync(null, null, null, cancellationToken);
        var code = $"REG-{DateTime.UtcNow:yyyyMMdd}-{(count + 1):D4}";
        _logger.LogInformation("Generated registration number: {RegistrationNumber}", code);
        return code;
    }

    public Task<bool> IsRegistrationAllowedAsync(Event evt, CancellationToken cancellationToken = default)
    {
        var allowed = evt.Status == EventStatus.RegistrationOpen &&
            DateTime.UtcNow >= evt.RegistrationOpenDate &&
            DateTime.UtcNow <= evt.RegistrationCloseDate;
        return Task.FromResult(allowed);
    }

    public async Task<bool> IsCapacityAvailableAsync(Event evt, CancellationToken cancellationToken = default)
    {
        if (!evt.MaxParticipants.HasValue) return true;
        var currentCount = await _registrationRepository.GetRegistrationCountAsync(evt.Id, cancellationToken);
        return currentCount < evt.MaxParticipants.Value;
    }

    public async Task<bool> IsDuplicateRegistrationAsync(Guid eventId, Guid? athleteId, Guid? userId, CancellationToken cancellationToken = default)
    {
        return await _registrationRepository.IsAlreadyRegisteredAsync(eventId, athleteId, userId, cancellationToken);
    }

    public async Task<int> GetCurrentRegistrationCountAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return await _registrationRepository.GetRegistrationCountAsync(eventId, cancellationToken);
    }

    public async Task<EventRegistration?> ProcessWaitlistPromotionAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var waitlisted = await _registrationRepository.GetByEventIdWithStatusAsync(eventId, EventRegistrationStatus.Waitlisted, cancellationToken);
        var next = waitlisted.OrderBy(r => r.WaitlistPosition).FirstOrDefault();
        if (next != null)
        {
            next.Status = EventRegistrationStatus.Approved;
            next.ApprovalDate = DateTime.UtcNow;
            _logger.LogInformation("Promoted from waitlist: {RegistrationNumber}", next.RegistrationNumber);
        }
        return next;
    }

    public Task<EventRegistrationStatus> DetermineInitialStatusAsync(Event evt, CancellationToken cancellationToken = default)
    {
        var status = evt.RegistrationType switch
        {
            EventRegistrationType.Free => EventRegistrationStatus.Approved,
            EventRegistrationType.ApprovalRequired => EventRegistrationStatus.Pending,
            EventRegistrationType.Waitlist => EventRegistrationStatus.Waitlisted,
            EventRegistrationType.Invitation => EventRegistrationStatus.Pending,
            EventRegistrationType.Paid => EventRegistrationStatus.Pending,
            _ => EventRegistrationStatus.Pending
        };
        return Task.FromResult(status);
    }
}
