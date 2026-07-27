using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Platform.Competition.Interfaces;
using SportsGurukul.Platform.Competition.Models;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Commands.StartMatch;

public class StartLiveMatchCommandHandler : IRequestHandler<StartLiveMatchCommand, Result<Guid>>
{
    private readonly ILiveScoringService _liveScoringService;
    private readonly IMatchLifecycleService _lifecycleService;
    private readonly ILogger<StartLiveMatchCommandHandler> _logger;

    public StartLiveMatchCommandHandler(
        ILiveScoringService liveScoringService,
        IMatchLifecycleService lifecycleService,
        ILogger<StartLiveMatchCommandHandler> logger)
    {
        _liveScoringService = liveScoringService;
        _lifecycleService = lifecycleService;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(StartLiveMatchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting live match {MatchId} for tournament {TournamentId}", request.MatchId, request.TournamentId);

        var match = await _liveScoringService.StartMatchAsync(
            request.TournamentId, request.MatchId, request.SportCode, cancellationToken);

        match.HomeParticipantId = request.HomeParticipantId;
        match.HomeParticipantName = request.HomeParticipantName;
        match.AwayParticipantId = request.AwayParticipantId;
        match.AwayParticipantName = request.AwayParticipantName;

        _logger.LogInformation("Live match started: {LiveMatchId}", match.Id);
        return Result<Guid>.Success(match.Id);
    }
}
