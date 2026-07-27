using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.SharedScheduling.Engine;
using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Queries.GetSchedulingConflicts;

public class GetSchedulingConflictsQueryHandler
    : IRequestHandler<GetSchedulingConflictsQuery, Result<IReadOnlyList<ConflictInfo>>>
{
    private readonly IConflictDetectionEngine _conflictDetectionEngine;
    private readonly ILogger<GetSchedulingConflictsQueryHandler> _logger;

    public GetSchedulingConflictsQueryHandler(
        IConflictDetectionEngine conflictDetectionEngine,
        ILogger<GetSchedulingConflictsQueryHandler> logger)
    {
        _conflictDetectionEngine = conflictDetectionEngine;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<ConflictInfo>>> Handle(
        GetSchedulingConflictsQuery request, CancellationToken cancellationToken)
    {
        var conflicts = await _conflictDetectionEngine.GetUnresolvedConflictsAsync(
            request.ResourceId, request.ResourceType, cancellationToken);

        _logger.LogInformation(
            "Retrieved {Count} unresolved conflicts for {ResourceType} {ResourceId}",
            conflicts.Count, request.ResourceType, request.ResourceId);

        return Result<IReadOnlyList<ConflictInfo>>.Success(conflicts);
    }
}
