using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Commands.ResolveSchedulingConflict;

public class ResolveSchedulingConflictCommand : IRequest<Result<bool>>
{
    public Guid ConflictId { get; set; }
    public string ResolutionNotes { get; set; } = string.Empty;
}
