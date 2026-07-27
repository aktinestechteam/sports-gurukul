using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.SendReminder;

public class SendReminderCommand : IRequest<Result<bool>>
{
    public Guid ReminderId { get; set; }
    public string? OverrideChannel { get; set; }
}
