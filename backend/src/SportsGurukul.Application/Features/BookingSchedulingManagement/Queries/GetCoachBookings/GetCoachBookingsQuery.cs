using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetCoachBookings;

public class GetCoachBookingsQuery : IRequest<Result<IReadOnlyList<BookingSummaryDto>>>
{
    public Guid CoachId { get; set; }
    public DateTime Date { get; set; }
}
