using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetBookingHistory;

public class GetBookingHistoryQuery : IRequest<Result<IReadOnlyList<BookingHistoryDto>>>
{
    public Guid BookingId { get; set; }
}
