using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.Competition.Interfaces;
using SportsGurukul.Platform.Competition.Models;

namespace SportsGurukul.Platform.Competition.Services;

public class StatisticsService : IStatisticsService
{
    private readonly MemoryMatchStore _store;
    private readonly ConcurrentDictionary<Guid, MatchStatistics> _matchStats = new();
    private readonly ConcurrentDictionary<Guid, PlayerStatistics> _playerStats = new();
    private readonly ConcurrentDictionary<Guid, TeamStatistics> _teamStats = new();
    private readonly ILogger<StatisticsService> _logger;

    public StatisticsService(MemoryMatchStore store, ILogger<StatisticsService> logger)
    {
        _store = store;
        _logger = logger;
    }

    public Task<MatchStatistics> GetMatchStatisticsAsync(Guid matchId, CancellationToken cancellationToken = default)
    {
        if (_matchStats.TryGetValue(matchId, out var stats))
            return Task.FromResult(stats);

        var match = _store.Get(matchId);
        if (match == null)
            return Task.FromResult(new MatchStatistics { MatchId = matchId });

        var result = BuildMatchStatistics(match);
        _matchStats[matchId] = result;
        return Task.FromResult(result);
    }

    public Task<PlayerStatistics> GetPlayerStatisticsAsync(Guid participantId, string? sportCode, CancellationToken cancellationToken = default)
    {
        var key = participantId;
        if (_playerStats.TryGetValue(key, out var stats))
            return Task.FromResult(stats);

        var allMatches = _store.GetByTournament(Guid.Empty)
            .Where(m => m.HomeParticipantId == participantId || m.AwayParticipantId == participantId)
            .ToList();

        var result = BuildPlayerStatistics(participantId, allMatches, sportCode);
        _playerStats[key] = result;
        return Task.FromResult(result);
    }

    public Task<TeamStatistics> GetTeamStatisticsAsync(Guid teamId, string? sportCode, CancellationToken cancellationToken = default)
    {
        if (_teamStats.TryGetValue(teamId, out var stats))
            return Task.FromResult(stats);

        var result = new TeamStatistics { TeamId = teamId, TeamName = "Team" };
        _teamStats[teamId] = result;
        return Task.FromResult(result);
    }

    public Task GenerateMatchStatisticsAsync(Guid matchId, CancellationToken cancellationToken = default)
    {
        var match = _store.Get(matchId);
        if (match == null) return Task.CompletedTask;

        var stats = BuildMatchStatistics(match);
        _matchStats[matchId] = stats;
        _logger.LogInformation("Match statistics generated for {MatchId}", matchId);
        return Task.CompletedTask;
    }

    public Task GeneratePlayerStatisticsAsync(Guid participantId, string? sportCode, CancellationToken cancellationToken = default)
    {
        var allMatches = _store.GetByTournament(Guid.Empty)
            .Where(m => m.HomeParticipantId == participantId || m.AwayParticipantId == participantId)
            .ToList();

        var stats = BuildPlayerStatistics(participantId, allMatches, sportCode);
        _playerStats[participantId] = stats;
        return Task.CompletedTask;
    }

    public Task GenerateTeamStatisticsAsync(Guid teamId, string? sportCode, CancellationToken cancellationToken = default)
    {
        var stats = new TeamStatistics { TeamId = teamId, TeamName = "Team" };
        _teamStats[teamId] = stats;
        return Task.CompletedTask;
    }

    private static MatchStatistics BuildMatchStatistics(LiveMatch match)
    {
        var events = match.ScoreEvents.Where(e => !e.IsUndo).ToList();
        var homeEvents = events.Where(e => e.ParticipantId == match.HomeParticipantId).ToList();
        var awayEvents = events.Where(e => e.ParticipantId == match.AwayParticipantId).ToList();

        return new MatchStatistics
        {
            MatchId = match.MatchId,
            SportCode = match.SportCode,
            HomeStatistics = new ParticipantStatistics
            {
                ParticipantId = match.HomeParticipantId,
                ParticipantName = match.HomeParticipantName,
                TotalPoints = match.HomeScore.TotalPoints
            },
            AwayStatistics = new ParticipantStatistics
            {
                ParticipantId = match.AwayParticipantId,
                ParticipantName = match.AwayParticipantName,
                TotalPoints = match.AwayScore.TotalPoints
            },
            Duration = match.TotalPlayTime,
            TotalEvents = events.Count,
            KeyHighlights = events.Where(e => e.Points >= 3).Select(e => $"{e.ParticipantName} scored {e.Points} ({e.Unit})").ToList()
        };
    }

    private static PlayerStatistics BuildPlayerStatistics(Guid participantId, List<LiveMatch> matches, string? sportCode)
    {
        var wins = matches.Count(m => m.WinnerId == participantId);
        var total = matches.Count;
        var losses = matches.Count(m => m.WinnerId.HasValue && m.WinnerId != participantId);
        var points = matches.Sum(m => m.HomeParticipantId == participantId ? m.HomeScore.TotalPoints : m.AwayParticipantId == participantId ? m.AwayScore.TotalPoints : 0);

        return new PlayerStatistics
        {
            ParticipantId = participantId,
            ParticipantName = matches.FirstOrDefault(m => m.HomeParticipantId == participantId)?.HomeParticipantName
                ?? matches.FirstOrDefault(m => m.AwayParticipantId == participantId)?.AwayParticipantName
                ?? "Unknown",
            SportCode = sportCode,
            MatchesPlayed = total,
            Wins = wins,
            Losses = losses,
            TotalPoints = points,
            AveragePointsPerMatch = total > 0 ? (decimal)points / total : 0,
            WinPercentage = total > 0 ? (decimal)wins / total * 100 : 0
        };
    }
}
