using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.ResolveBookingConflict;

public class ResolveBookingConflictCommand : IRequest<Result<bool>>
{
    public Guid ConflictId { get; set; }
    public string ResolutionNotes { get; set; } = string.Empty;
}
