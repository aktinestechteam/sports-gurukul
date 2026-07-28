using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Services;

public interface IEventAttendanceService
{
    Task<bool> CanCheckInAsync(EventParticipant participant, CancellationToken cancellationToken = default);
    Task<bool> CanCheckOutAsync(EventParticipant participant, EventAttendance attendance, CancellationToken cancellationToken = default);
    Task<EventAttendanceStatus> CalculateAttendanceStatusAsync(EventParticipant participant, CancellationToken cancellationToken = default);
    Task<double> CalculateAttendanceRateAsync(Guid eventId, CancellationToken cancellationToken = default);
}
