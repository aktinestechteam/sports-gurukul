using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.Competition.Interfaces;
using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Services;

public class LeaderboardService : ILeaderboardService
{
    private readonly IRankingService _rankingService;
    private readonly IStandingsService _standingsService;
    private readonly ILogger<LeaderboardService> _logger;

    public LeaderboardService(IRankingService rankingService, IStandingsService standingsService, ILogger<LeaderboardService> logger)
    {
        _rankingService = rankingService;
        _standingsService = standingsService;
        _logger = logger;
    }

    public async Task<Leaderboard> GenerateLeaderboardAsync(Guid tournamentId, LeaderboardType type, string? sportCode, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<LeaderboardEntry> entries;

        if (type == LeaderboardType.Tournament)
        {
            var standings = await _standingsService.GetTournamentStandingsAsync(tournamentId, sportCode, cancellationToken);
            entries = standings.Select(s => new LeaderboardEntry
            {
                Position = s.Position,
                ParticipantId = s.ParticipantId,
                ParticipantName = s.ParticipantName,
                AcademyName = s.AcademyName,
                Points = s.Points,
                Wins = s.Won,
                Losses = s.Lost,
                Draws = s.Drawn,
                MatchesPlayed = s.Played,
                WinPercentage = s.Played > 0 ? (decimal)s.Won / s.Played * 100 : 0,
                GoalDifference = s.GoalDifference,
                GoalsFor = s.GoalsFor,
                GoalsAgainst = s.GoalsAgainst
            }).ToList();
        }
        else
        {
            entries = await _rankingService.CalculateRankingsAsync(tournamentId, sportCode, cancellationToken);
        }

        var leaderboard = new Leaderboard
        {
            Id = Guid.NewGuid(),
            TournamentId = tournamentId,
            Type = type,
            SportCode = sportCode,
            Entries = entries.ToList(),
            GeneratedAt = DateTime.UtcNow,
            Version = 1
        };

        _logger.LogInformation("Leaderboard generated for tournament {TournamentId}, type {Type}", tournamentId, type);
        return leaderboard;
    }

    public Task<Leaderboard?> GetLeaderboardAsync(Guid tournamentId, LeaderboardType type, string? sportCode, CancellationToken cancellationToken = default)
    {
        _ = tournamentId;
        _ = type;
        _ = sportCode;
        return Task.FromResult<Leaderboard?>(null);
    }

    public async Task UpdateLeaderboardAfterMatchAsync(Guid tournamentId, LeaderboardType type, string? sportCode, CancellationToken cancellationToken = default)
    {
        await GenerateLeaderboardAsync(tournamentId, type, sportCode, cancellationToken);
    }
}
