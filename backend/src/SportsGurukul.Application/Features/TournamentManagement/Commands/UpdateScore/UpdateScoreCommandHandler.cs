using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.Services;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.UpdateScore;

public class UpdateScoreCommandHandler : IRequestHandler<UpdateScoreCommand, Result<Unit>>
{
    private readonly IMatchRepository _matchRepository;
    private readonly IScoringService _scoringService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateScoreCommandHandler> _logger;

    public UpdateScoreCommandHandler(
        IMatchRepository matchRepository,
        IScoringService scoringService,
        IUnitOfWork unitOfWork,
        ILogger<UpdateScoreCommandHandler> logger)
    {
        _matchRepository = matchRepository;
        _scoringService = scoringService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(UpdateScoreCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating score for match: {MatchId}", request.MatchId);

        var match = await _matchRepository.GetByIdAsync(request.MatchId, cancellationToken);
        if (match is null)
            return Result<Unit>.Failure("Match not found.");

        if (match.Status != MatchStatus.InProgress)
            return Result<Unit>.Failure("Score can only be updated for in-progress matches.");

        await _scoringService.UpdateScoreAsync(match, request.HomeScore, request.AwayScore, request.ScoreDetails, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Score updated for match: {MatchId}, Home: {HomeScore}, Away: {AwayScore}", request.MatchId, request.HomeScore, request.AwayScore);
        return Result<Unit>.Success(Unit.Value);
    }
}
