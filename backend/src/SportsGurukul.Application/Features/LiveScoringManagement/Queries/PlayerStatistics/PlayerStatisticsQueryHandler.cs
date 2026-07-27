using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.LiveScoringManagement.DTOs;
using SportsGurukul.Platform.Competition.Interfaces;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Queries.PlayerStatistics;

public class PlayerStatisticsQueryHandler : IRequestHandler<PlayerStatisticsQuery, Result<PlayerStatisticsDto>>
{
    private readonly IStatisticsService _statisticsService;
    private readonly ILogger<PlayerStatisticsQueryHandler> _logger;

    public PlayerStatisticsQueryHandler(IStatisticsService statisticsService, ILogger<PlayerStatisticsQueryHandler> logger)
    {
        _statisticsService = statisticsService;
        _logger = logger;
    }

    public async Task<Result<PlayerStatisticsDto>> Handle(PlayerStatisticsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting player statistics for {PlayerId}", request.PlayerId);

        var stats = await _statisticsService.GetPlayerStatisticsAsync(request.PlayerId, request.SportCode, cancellationToken);

        var dto = new PlayerStatisticsDto
        {
            ParticipantId = stats.ParticipantId,
            ParticipantName = stats.ParticipantName,
            SportCode = stats.SportCode,
            MatchesPlayed = stats.MatchesPlayed,
            Wins = stats.Wins,
            Losses = stats.Losses,
            Draws = stats.Draws,
            WinPercentage = stats.WinPercentage,
            TotalPoints = stats.TotalPoints,
            AveragePointsPerMatch = stats.AveragePointsPerMatch,
            BestScore = stats.BestScore,
            WorstScore = stats.WorstScore,
            CurrentStreak = stats.CurrentStreak,
            StreakType = stats.StreakType,
            RecentPerformances = stats.RecentPerformances.Select(p => new MatchPerformanceDto
            {
                MatchId = p.MatchId,
                MatchDate = p.MatchDate,
                OpponentName = p.OpponentName,
                PointsScored = p.PointsScored,
                IsWin = p.IsWin,
                IsDraw = p.IsDraw
            }).ToList()
        };

        return Result<PlayerStatisticsDto>.Success(dto);
    }
}
