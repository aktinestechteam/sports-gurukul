using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Platform.Competition.Interfaces;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Commands.UndoScore;

public class UndoScoreCommandHandler : IRequestHandler<UndoScoreCommand, Result<Unit>>
{
    private readonly ILiveScoringService _liveScoringService;
    private readonly ILogger<UndoScoreCommandHandler> _logger;

    public UndoScoreCommandHandler(ILiveScoringService liveScoringService, ILogger<UndoScoreCommandHandler> logger)
    {
        _liveScoringService = liveScoringService;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(UndoScoreCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Undoing last score for match {MatchId}", request.MatchId);
        await _liveScoringService.UndoLastScoreAsync(request.MatchId, cancellationToken);
        return Result<Unit>.Success(Unit.Value);
    }
}
