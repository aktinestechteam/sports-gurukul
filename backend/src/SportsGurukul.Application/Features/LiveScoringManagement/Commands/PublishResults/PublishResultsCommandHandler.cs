using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Platform.Competition.Interfaces;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Commands.PublishResults;

public class PublishResultsCommandHandler : IRequestHandler<PublishResultsCommand, Result<Unit>>
{
    private readonly IMatchLifecycleService _lifecycleService;
    private readonly ILogger<PublishResultsCommandHandler> _logger;

    public PublishResultsCommandHandler(IMatchLifecycleService lifecycleService, ILogger<PublishResultsCommandHandler> logger)
    {
        _lifecycleService = lifecycleService;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(PublishResultsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing results for match {MatchId} in tournament {TournamentId}", request.MatchId, request.TournamentId);
        return Result<Unit>.Success(Unit.Value);
    }
}
