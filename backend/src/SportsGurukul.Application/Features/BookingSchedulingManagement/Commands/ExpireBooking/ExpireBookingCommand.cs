using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.ExpireBooking;

public class ExpireBookingCommand : IRequest<Result<BookingDto>>
{
    public Guid BookingId { get; set; }
}
