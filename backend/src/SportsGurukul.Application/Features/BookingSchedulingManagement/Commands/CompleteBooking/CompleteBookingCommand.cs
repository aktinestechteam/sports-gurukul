using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CompleteBooking;

public class CompleteBookingCommand : IRequest<Result<BookingDto>>
{
    public Guid BookingId { get; set; }
}
