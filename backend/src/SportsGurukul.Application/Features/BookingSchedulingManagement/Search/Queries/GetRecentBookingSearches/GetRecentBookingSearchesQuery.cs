using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Queries.GetRecentBookingSearches;

public class GetRecentBookingSearchesQuery : IRequest<Result<IReadOnlyList<RecentBookingSearchDto>>>
{
    public Guid UserId { get; set; }
    public int Limit { get; set; } = 10;
}
