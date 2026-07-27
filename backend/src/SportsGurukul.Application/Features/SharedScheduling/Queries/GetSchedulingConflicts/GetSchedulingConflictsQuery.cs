using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Queries.GetSchedulingConflicts;

public class GetSchedulingConflictsQuery : IRequest<Result<IReadOnlyList<ConflictInfo>>>
{
    public Guid ResourceId { get; set; }
    public string ResourceType { get; set; } = string.Empty;
}
