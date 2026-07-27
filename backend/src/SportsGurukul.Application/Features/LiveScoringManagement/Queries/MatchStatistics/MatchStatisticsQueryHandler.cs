using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.LiveScoringManagement.DTOs;
using SportsGurukul.Platform.Competition.Interfaces;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Queries.MatchStatistics;

public class MatchStatisticsQueryHandler : IRequestHandler<MatchStatisticsQuery, Result<MatchStatisticsDto>>
{
    private readonly IStatisticsService _statisticsService;
    private readonly ILogger<MatchStatisticsQueryHandler> _logger;

    public MatchStatisticsQueryHandler(IStatisticsService statisticsService, ILogger<MatchStatisticsQueryHandler> logger)
    {
        _statisticsService = statisticsService;
        _logger = logger;
    }

    public async Task<Result<MatchStatisticsDto>> Handle(MatchStatisticsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting match statistics for {MatchId}", request.MatchId);

        var stats = await _statisticsService.GetMatchStatisticsAsync(request.MatchId, cancellationToken);

        var dto = new MatchStatisticsDto
        {
            MatchId = stats.MatchId,
            SportCode = stats.SportCode,
            HomeStatistics = new ParticipantStatisticsDto
            {
                ParticipantId = stats.HomeStatistics.ParticipantId,
                ParticipantName = stats.HomeStatistics.ParticipantName,
                TotalPoints = stats.HomeStatistics.TotalPoints,
                PointsPerPeriod = stats.HomeStatistics.PointsPerPeriod,
                Fouls = stats.HomeStatistics.Fouls,
                Timeouts = stats.HomeStatistics.Timeouts,
                Substitutions = stats.HomeStatistics.Substitutions,
                AverageScore = stats.HomeStatistics.AverageScore,
                PossessionPercentage = stats.HomeStatistics.PossessionPercentage
            },
            AwayStatistics = new ParticipantStatisticsDto
            {
                ParticipantId = stats.AwayStatistics.ParticipantId,
                ParticipantName = stats.AwayStatistics.ParticipantName,
                TotalPoints = stats.AwayStatistics.TotalPoints,
                PointsPerPeriod = stats.AwayStatistics.PointsPerPeriod,
                Fouls = stats.AwayStatistics.Fouls,
                Timeouts = stats.AwayStatistics.Timeouts,
                Substitutions = stats.AwayStatistics.Substitutions,
                AverageScore = stats.AwayStatistics.AverageScore,
                PossessionPercentage = stats.AwayStatistics.PossessionPercentage
            },
            Duration = stats.Duration,
            TotalEvents = stats.TotalEvents,
            KeyHighlights = stats.KeyHighlights,
            PeriodStats = stats.PeriodStats.Select(p => new PeriodStatisticsDto
            {
                PeriodNumber = p.PeriodNumber,
                PeriodName = p.PeriodName,
                HomeScore = p.HomeScore,
                AwayScore = p.AwayScore,
                Duration = p.Duration,
                HomeEvents = p.HomeEvents,
                AwayEvents = p.AwayEvents
            }).ToList()
        };

        return Result<MatchStatisticsDto>.Success(dto);
    }
}
