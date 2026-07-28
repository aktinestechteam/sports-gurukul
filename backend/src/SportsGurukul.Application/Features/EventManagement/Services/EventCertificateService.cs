using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Services;

public class EventCertificateService : IEventCertificateService
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventAttendanceRepository _attendanceRepository;
    private readonly ILogger<EventCertificateService> _logger;

    public EventCertificateService(
        IEventRepository eventRepository,
        IEventAttendanceRepository attendanceRepository,
        ILogger<EventCertificateService> logger)
    {
        _eventRepository = eventRepository;
        _attendanceRepository = attendanceRepository;
        _logger = logger;
    }

    public async Task<string> GenerateCertificateNumberAsync(CancellationToken cancellationToken = default)
    {
        var code = $"CERT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
        _logger.LogInformation("Generated certificate number: {CertificateNumber}", code);
        return code;
    }

    public async Task<bool> IsEligibleForCertificateAsync(EventParticipant participant, CancellationToken cancellationToken = default)
    {
        if (participant.Event.Status != EventStatus.Completed) return false;
        var attendances = await _attendanceRepository.GetByParticipantIdAsync(participant.Id, cancellationToken);
        if (attendances.Count == 0) return false;
        var attendedCount = attendances.Count(a => a.Status is EventAttendanceStatus.Present or EventAttendanceStatus.CheckedIn);
        var rate = (double)attendedCount / attendances.Count;
        return rate >= 0.75;
    }

    public async Task<IReadOnlyList<EventParticipant>> GetEligibleParticipantsAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var allAttendances = await _attendanceRepository.GetByEventIdAsync(eventId, cancellationToken);
        var eligibleIds = allAttendances
            .GroupBy(a => a.ParticipantId)
            .Where(g => g.Any(a => a.Status is EventAttendanceStatus.Present or EventAttendanceStatus.CheckedIn))
            .Select(g => g.Key)
            .ToList();

        return allAttendances
            .Where(a => eligibleIds.Contains(a.ParticipantId))
            .Select(a => a.Participant)
            .Distinct()
            .ToList();
    }
}
