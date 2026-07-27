using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Platform.Competition.Interfaces;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Commands.UpdateLiveScore;

public class UpdateLiveScoreCommandHandler : IRequestHandler<UpdateLiveScoreCommand, Result<Unit>>
{
    private readonly ILiveScoringService _liveScoringService;
    private readonly IRankingService _rankingService;
    private readonly IStandingsService _standingsService;
    private readonly ILogger<UpdateLiveScoreCommandHandler> _logger;

    public UpdateLiveScoreCommandHandler(
        ILiveScoringService liveScoringService,
        IRankingService rankingService,
        IStandingsService standingsService,
        ILogger<UpdateLiveScoreCommandHandler> logger)
    {
        _liveScoringService = liveScoringService;
        _rankingService = rankingService;
        _standingsService = standingsService;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(UpdateLiveScoreCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating live score for match {MatchId}: {Points} by {Participant}",
            request.MatchId, request.Points, request.ParticipantId);

        var match = await _liveScoringService.UpdateScoreAsync(
            request.MatchId, request.ParticipantId, request.Points,
            request.Unit, request.PeriodNumber, request.Description, cancellationToken);

        await _rankingService.UpdateRankingsAfterMatchAsync(
            match.TournamentId, match.HomeParticipantId, match.AwayParticipantId,
            match.HomeScore.TotalPoints, match.AwayScore.TotalPoints, cancellationToken);

        await _standingsService.UpdateStandingsAfterMatchAsync(
            match.TournamentId, match.HomeParticipantId, match.AwayParticipantId,
            match.HomeScore.TotalPoints, match.AwayScore.TotalPoints, cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}
