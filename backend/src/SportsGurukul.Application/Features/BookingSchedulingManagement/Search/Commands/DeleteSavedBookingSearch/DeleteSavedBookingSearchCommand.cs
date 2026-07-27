using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Commands.DeleteSavedBookingSearch;

public class DeleteSavedBookingSearchCommand : IRequest<Result<Unit>>
{
    public Guid UserId { get; set; }
    public Guid SavedSearchId { get; set; }
}
