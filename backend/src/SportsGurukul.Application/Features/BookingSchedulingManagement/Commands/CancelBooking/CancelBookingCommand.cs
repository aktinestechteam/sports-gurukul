using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CancelBooking;

public class CancelBookingCommand : IRequest<Result<BookingDto>>
{
    public Guid BookingId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
