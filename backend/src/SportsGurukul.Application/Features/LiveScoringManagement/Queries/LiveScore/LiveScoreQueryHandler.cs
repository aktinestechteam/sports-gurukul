using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.LiveScoringManagement.DTOs;
using SportsGurukul.Platform.Competition.Interfaces;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Queries.LiveScore;

public class LiveScoreQueryHandler : IRequestHandler<LiveScoreQuery, Result<LiveScoreDto>>
{
    private readonly ILiveScoringService _liveScoringService;
    private readonly ILogger<LiveScoreQueryHandler> _logger;

    public LiveScoreQueryHandler(ILiveScoringService liveScoringService, ILogger<LiveScoreQueryHandler> logger)
    {
        _liveScoringService = liveScoringService;
        _logger = logger;
    }

    public async Task<Result<LiveScoreDto>> Handle(LiveScoreQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting live score for match {MatchId}", request.MatchId);

        var match = await _liveScoringService.GetLiveMatchAsync(request.MatchId, cancellationToken);
        if (match == null)
            return Result<LiveScoreDto>.Failure("Live match not found.");

        var dto = new LiveScoreDto
        {
            MatchId = match.MatchId,
            LiveMatchId = match.Id,
            SportCode = match.SportCode,
            Status = match.Status.ToString(),
            HomeParticipantId = match.HomeParticipantId,
            HomeParticipantName = match.HomeParticipantName,
            AwayParticipantId = match.AwayParticipantId,
            AwayParticipantName = match.AwayParticipantName,
            HomeScore = match.HomeScore.TotalPoints,
            AwayScore = match.AwayScore.TotalPoints,
            HomeSets = match.HomeScore.Sets,
            AwaySets = match.AwayScore.Sets,
            HomeGames = match.HomeScore.Games,
            AwayGames = match.AwayScore.Games,
            CurrentPeriod = match.CurrentPeriod,
            CurrentPeriodName = match.CurrentPeriodName,
            StartedAt = match.StartedAt,
            TotalPlayTime = match.TotalPlayTime,
            WinnerId = match.WinnerId,
            WinnerName = match.WinnerName,
            Version = match.Version,
            Events = match.ScoreEvents.Select(e => new ScoreEventDto
            {
                Id = e.Id,
                ParticipantId = e.ParticipantId,
                ParticipantName = e.ParticipantName,
                Unit = e.Unit.ToString(),
                Points = e.Points,
                PeriodNumber = e.PeriodNumber,
                Description = e.Description,
                Timestamp = e.Timestamp,
                IsUndo = e.IsUndo
            }).ToList()
        };

        return Result<LiveScoreDto>.Success(dto);
    }
}
