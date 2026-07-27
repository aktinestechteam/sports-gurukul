using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.Services;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.RecordForfeit;

public class RecordForfeitCommandHandler : IRequestHandler<RecordForfeitCommand, Result<Unit>>
{
    private readonly IMatchRepository _matchRepository;
    private readonly IScoringService _scoringService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RecordForfeitCommandHandler> _logger;

    public RecordForfeitCommandHandler(
        IMatchRepository matchRepository,
        IScoringService scoringService,
        IUnitOfWork unitOfWork,
        ILogger<RecordForfeitCommandHandler> logger)
    {
        _matchRepository = matchRepository;
        _scoringService = scoringService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(RecordForfeitCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recording forfeit for match: {MatchId}", request.MatchId);

        var match = await _matchRepository.GetByIdAsync(request.MatchId, cancellationToken);
        if (match is null)
            return Result<Unit>.Failure("Match not found.");

        await _scoringService.RecordForfeitAsync(match, request.WinnerId, request.Notes, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Forfeit recorded for match: {MatchId}", request.MatchId);
        return Result<Unit>.Success(Unit.Value);
    }
}
