using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Queries.GetSavedBookingSearches;

public class GetSavedBookingSearchesQuery : IRequest<Result<IReadOnlyList<SavedBookingSearchDto>>>
{
    public Guid UserId { get; set; }
}
