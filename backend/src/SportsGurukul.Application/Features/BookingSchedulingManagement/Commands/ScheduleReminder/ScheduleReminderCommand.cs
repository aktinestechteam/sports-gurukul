using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.ScheduleReminder;

public class ScheduleReminderCommand : IRequest<Result<ReminderDto>>
{
    public Guid BookingId { get; set; }
    public int ReminderMinutesBefore { get; set; }
    public string? Channel { get; set; }
    public string? Notes { get; set; }
}
