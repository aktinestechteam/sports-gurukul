using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.RejectBooking;

public class RejectBookingCommand : IRequest<Result<BookingDto>>
{
    public Guid BookingId { get; set; }
    public string? Reason { get; set; }
}
