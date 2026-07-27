using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CancelReminder;

public class CancelReminderCommand : IRequest<Result<bool>>
{
    public Guid ReminderId { get; set; }
}
