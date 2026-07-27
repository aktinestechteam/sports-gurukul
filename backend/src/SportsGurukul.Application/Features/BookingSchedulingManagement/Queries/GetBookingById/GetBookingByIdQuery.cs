using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetBookingById;

public class GetBookingByIdQuery : IRequest<Result<BookingDto>>
{
    public Guid BookingId { get; set; }
}
