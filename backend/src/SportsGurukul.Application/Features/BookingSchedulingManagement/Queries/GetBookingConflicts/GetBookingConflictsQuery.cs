using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetBookingConflicts;

public class GetBookingConflictsQuery : IRequest<Result<IReadOnlyList<BookingConflictDto>>>
{
    public Guid BookingId { get; set; }
}
