using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Services;

public class EventAttendanceService : IEventAttendanceService
{
    private readonly IEventAttendanceRepository _attendanceRepository;
    private readonly ILogger<EventAttendanceService> _logger;

    public EventAttendanceService(IEventAttendanceRepository attendanceRepository, ILogger<EventAttendanceService> logger)
    {
        _attendanceRepository = attendanceRepository;
        _logger = logger;
    }

    public Task<bool> CanCheckInAsync(EventParticipant participant, CancellationToken cancellationToken = default)
    {
        var allowed = participant.AttendanceStatus is EventAttendanceStatus.Registered or EventAttendanceStatus.Late;
        return Task.FromResult(allowed);
    }

    public Task<bool> CanCheckOutAsync(EventParticipant participant, EventAttendance attendance, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(attendance.CheckInTime.HasValue && !attendance.CheckOutTime.HasValue);
    }

    public Task<EventAttendanceStatus> CalculateAttendanceStatusAsync(EventParticipant participant, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(participant.AttendanceStatus);
    }

    public async Task<double> CalculateAttendanceRateAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var totalParticipants = await _attendanceRepository.GetAttendeeCountAsync(eventId, cancellationToken);
        if (totalParticipants == 0) return 0;
        var present = await _attendanceRepository.GetByStatusAsync(eventId, EventAttendanceStatus.Present, cancellationToken);
        return Math.Round((double)present.Count / totalParticipants * 100, 2);
    }
}
