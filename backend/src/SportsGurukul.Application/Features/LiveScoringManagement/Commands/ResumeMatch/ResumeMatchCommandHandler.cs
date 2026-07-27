using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Platform.Competition.Interfaces;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Commands.ResumeMatch;

public class ResumeMatchCommandHandler : IRequestHandler<ResumeMatchCommand, Result<Unit>>
{
    private readonly IMatchLifecycleService _lifecycleService;
    private readonly ILogger<ResumeMatchCommandHandler> _logger;

    public ResumeMatchCommandHandler(IMatchLifecycleService lifecycleService, ILogger<ResumeMatchCommandHandler> logger)
    {
        _lifecycleService = lifecycleService;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(ResumeMatchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Resuming match {MatchId}", request.MatchId);
        await _lifecycleService.TransitionToLiveAsync(request.MatchId, cancellationToken);
        return Result<Unit>.Success(Unit.Value);
    }
}
