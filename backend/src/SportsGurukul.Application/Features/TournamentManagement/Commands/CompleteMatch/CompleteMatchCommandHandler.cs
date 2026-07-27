using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.Services;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.CompleteMatch;

public class CompleteMatchCommandHandler : IRequestHandler<CompleteMatchCommand, Result<Unit>>
{
    private readonly IMatchRepository _matchRepository;
    private readonly IScoringService _scoringService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CompleteMatchCommandHandler> _logger;

    public CompleteMatchCommandHandler(
        IMatchRepository matchRepository,
        IScoringService scoringService,
        IUnitOfWork unitOfWork,
        ILogger<CompleteMatchCommandHandler> logger)
    {
        _matchRepository = matchRepository;
        _scoringService = scoringService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(CompleteMatchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Completing match: {MatchId}", request.MatchId);

        var match = await _matchRepository.GetByIdAsync(request.MatchId, cancellationToken);
        if (match is null)
            return Result<Unit>.Failure("Match not found.");

        if (match.Status != MatchStatus.InProgress)
            return Result<Unit>.Failure("Only in-progress matches can be completed.");

        if (request.WinnerId.HasValue)
            match.WinnerId = request.WinnerId.Value;

        await _scoringService.CompleteMatchAsync(match, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Match completed: {MatchId}", request.MatchId);
        return Result<Unit>.Success(Unit.Value);
    }
}
