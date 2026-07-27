using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Platform.Competition.Interfaces;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Commands.PauseMatch;

public class PauseMatchCommandHandler : IRequestHandler<PauseMatchCommand, Result<Unit>>
{
    private readonly IMatchLifecycleService _lifecycleService;
    private readonly ILogger<PauseMatchCommandHandler> _logger;

    public PauseMatchCommandHandler(IMatchLifecycleService lifecycleService, ILogger<PauseMatchCommandHandler> logger)
    {
        _lifecycleService = lifecycleService;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(PauseMatchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Pausing match {MatchId}", request.MatchId);
        await _lifecycleService.TransitionToPausedAsync(request.MatchId, cancellationToken);
        return Result<Unit>.Success(Unit.Value);
    }
}
