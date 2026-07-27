using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Platform.Competition.Interfaces;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Commands.CompleteMatch;

public class CompleteMatchCommandHandler : IRequestHandler<CompleteMatchCommand, Result<Unit>>
{
    private readonly IMatchLifecycleService _lifecycleService;
    private readonly ILogger<CompleteMatchCommandHandler> _logger;

    public CompleteMatchCommandHandler(IMatchLifecycleService lifecycleService, ILogger<CompleteMatchCommandHandler> logger)
    {
        _lifecycleService = lifecycleService;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(CompleteMatchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Completing match {MatchId}", request.MatchId);
        await _lifecycleService.TransitionToCompletedAsync(request.MatchId, request.WinnerId, request.WinnerName, cancellationToken);
        return Result<Unit>.Success(Unit.Value);
    }
}
