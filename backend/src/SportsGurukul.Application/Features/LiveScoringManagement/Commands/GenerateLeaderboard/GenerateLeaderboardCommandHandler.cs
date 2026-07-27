using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Platform.Competition.Interfaces;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Commands.GenerateLeaderboard;

public class GenerateLeaderboardCommandHandler : IRequestHandler<GenerateLeaderboardCommand, Result<Unit>>
{
    private readonly ILeaderboardService _leaderboardService;
    private readonly ILogger<GenerateLeaderboardCommandHandler> _logger;

    public GenerateLeaderboardCommandHandler(ILeaderboardService leaderboardService, ILogger<GenerateLeaderboardCommandHandler> logger)
    {
        _leaderboardService = leaderboardService;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(GenerateLeaderboardCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating leaderboard for tournament {TournamentId}, type {Type}", request.TournamentId, request.Type);
        await _leaderboardService.GenerateLeaderboardAsync(request.TournamentId, request.Type, request.SportCode, cancellationToken);
        return Result<Unit>.Success(Unit.Value);
    }
}
