using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.RescheduleBooking;

public class RescheduleBookingCommand : IRequest<Result<BookingDto>>
{
    public Guid BookingId { get; set; }
    public DateTime NewDate { get; set; }
    public TimeSpan NewStartTime { get; set; }
    public TimeSpan NewEndTime { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}
