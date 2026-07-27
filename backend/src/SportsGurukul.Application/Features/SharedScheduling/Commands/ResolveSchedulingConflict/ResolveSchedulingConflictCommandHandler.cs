using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.SharedScheduling.Engine;

namespace SportsGurukul.Application.Features.SharedScheduling.Commands.ResolveSchedulingConflict;

public class ResolveSchedulingConflictCommandHandler
    : IRequestHandler<ResolveSchedulingConflictCommand, Result<bool>>
{
    private readonly IConflictDetectionEngine _conflictDetectionEngine;
    private readonly ILogger<ResolveSchedulingConflictCommandHandler> _logger;

    public ResolveSchedulingConflictCommandHandler(
        IConflictDetectionEngine conflictDetectionEngine,
        ILogger<ResolveSchedulingConflictCommandHandler> logger)
    {
        _conflictDetectionEngine = conflictDetectionEngine;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(
        ResolveSchedulingConflictCommand request, CancellationToken cancellationToken)
    {
        var resolved = await _conflictDetectionEngine.ResolveConflictAsync(
            request.ConflictId, request.ResolutionNotes, cancellationToken);

        if (!resolved)
        {
            _logger.LogWarning("Failed to resolve conflict {ConflictId}", request.ConflictId);
            return Result<bool>.Failure($"Conflict {request.ConflictId} could not be resolved.");
        }

        _logger.LogInformation("Conflict {ConflictId} resolved successfully", request.ConflictId);
        return Result<bool>.Success(true);
    }
}
