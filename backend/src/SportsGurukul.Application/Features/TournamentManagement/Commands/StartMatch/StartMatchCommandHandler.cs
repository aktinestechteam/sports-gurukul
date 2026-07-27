using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.Services;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.StartMatch;

public class StartMatchCommandHandler : IRequestHandler<StartMatchCommand, Result<Unit>>
{
    private readonly IMatchRepository _matchRepository;
    private readonly IScoringService _scoringService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StartMatchCommandHandler> _logger;

    public StartMatchCommandHandler(
        IMatchRepository matchRepository,
        IScoringService scoringService,
        IUnitOfWork unitOfWork,
        ILogger<StartMatchCommandHandler> logger)
    {
        _matchRepository = matchRepository;
        _scoringService = scoringService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(StartMatchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting match: {MatchId}", request.MatchId);

        var match = await _matchRepository.GetByIdAsync(request.MatchId, cancellationToken);
        if (match is null)
            return Result<Unit>.Failure("Match not found.");

        if (match.Status != MatchStatus.Scheduled)
            return Result<Unit>.Failure("Only scheduled matches can be started.");

        await _scoringService.StartMatchAsync(match, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Match started: {MatchId}", request.MatchId);
        return Result<Unit>.Success(Unit.Value);
    }
}
