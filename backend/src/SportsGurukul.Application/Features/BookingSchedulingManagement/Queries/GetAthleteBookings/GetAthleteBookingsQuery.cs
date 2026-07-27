using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetAthleteBookings;

public class GetAthleteBookingsQuery : IRequest<Result<IReadOnlyList<BookingSummaryDto>>>
{
    public Guid AthleteId { get; set; }
}
