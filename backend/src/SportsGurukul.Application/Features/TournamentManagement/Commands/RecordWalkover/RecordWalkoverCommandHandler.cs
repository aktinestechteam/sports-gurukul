using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.Services;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.RecordWalkover;

public class RecordWalkoverCommandHandler : IRequestHandler<RecordWalkoverCommand, Result<Unit>>
{
    private readonly IMatchRepository _matchRepository;
    private readonly IScoringService _scoringService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RecordWalkoverCommandHandler> _logger;

    public RecordWalkoverCommandHandler(
        IMatchRepository matchRepository,
        IScoringService scoringService,
        IUnitOfWork unitOfWork,
        ILogger<RecordWalkoverCommandHandler> logger)
    {
        _matchRepository = matchRepository;
        _scoringService = scoringService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(RecordWalkoverCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recording walkover for match: {MatchId}", request.MatchId);

        var match = await _matchRepository.GetByIdAsync(request.MatchId, cancellationToken);
        if (match is null)
            return Result<Unit>.Failure("Match not found.");

        await _scoringService.RecordWalkoverAsync(match, request.WinnerId, request.Notes, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Walkover recorded for match: {MatchId}", request.MatchId);
        return Result<Unit>.Success(Unit.Value);
    }
}
