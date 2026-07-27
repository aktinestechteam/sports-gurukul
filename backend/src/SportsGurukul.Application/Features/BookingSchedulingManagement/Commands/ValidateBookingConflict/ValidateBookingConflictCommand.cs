using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.ValidateBookingConflict;

public class ValidateBookingConflictCommand : IRequest<Result<IReadOnlyList<BookingConflictDto>>>
{
    public Guid BookingId { get; set; }
}
