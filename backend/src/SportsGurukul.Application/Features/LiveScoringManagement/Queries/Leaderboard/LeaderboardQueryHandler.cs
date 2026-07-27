using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.LiveScoringManagement.DTOs;
using SportsGurukul.Platform.Competition.Interfaces;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Queries.Leaderboard;

public class LeaderboardQueryHandler : IRequestHandler<LeaderboardQuery, Result<LeaderboardDto>>
{
    private readonly ILeaderboardService _leaderboardService;
    private readonly ILogger<LeaderboardQueryHandler> _logger;

    public LeaderboardQueryHandler(ILeaderboardService leaderboardService, ILogger<LeaderboardQueryHandler> logger)
    {
        _leaderboardService = leaderboardService;
        _logger = logger;
    }

    public async Task<Result<LeaderboardDto>> Handle(LeaderboardQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting leaderboard for tournament {TournamentId}", request.TournamentId);

        var leaderboard = await _leaderboardService.GenerateLeaderboardAsync(
            request.TournamentId, request.Type, request.SportCode, cancellationToken);

        var dto = new LeaderboardDto
        {
            TournamentId = leaderboard.TournamentId,
            Type = leaderboard.Type.ToString(),
            SportCode = leaderboard.SportCode,
            GeneratedAt = leaderboard.GeneratedAt,
            Entries = leaderboard.Entries.Select(e => new LeaderboardEntryDto
            {
                Position = e.Position,
                ParticipantId = e.ParticipantId,
                ParticipantName = e.ParticipantName,
                AcademyName = e.AcademyName,
                Points = e.Points,
                Wins = e.Wins,
                Losses = e.Losses,
                Draws = e.Draws,
                MatchesPlayed = e.MatchesPlayed,
                WinPercentage = e.WinPercentage,
                GoalDifference = e.GoalDifference
            }).ToList()
        };

        return Result<LeaderboardDto>.Success(dto);
    }
}
