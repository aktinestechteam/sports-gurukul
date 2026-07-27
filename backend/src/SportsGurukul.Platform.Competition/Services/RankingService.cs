using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.Competition.Interfaces;
using SportsGurukul.Platform.Competition.Models;

namespace SportsGurukul.Platform.Competition.Services;

public class RankingService : IRankingService
{
    private readonly ConcurrentDictionary<Guid, List<LeaderboardEntry>> _rankings = new();
    private readonly ILogger<RankingService> _logger;

    public RankingService(ILogger<RankingService> logger)
    {
        _logger = logger;
    }

    public Task<IReadOnlyList<LeaderboardEntry>> CalculateRankingsAsync(Guid tournamentId, string? sportCode, CancellationToken cancellationToken = default)
    {
        var entries = _rankings.TryGetValue(tournamentId, out var list) ? list : new List<LeaderboardEntry>();
        var sorted = entries.OrderByDescending(e => e.Points).ThenByDescending(e => e.Wins).ThenByDescending(e => e.GoalDifference).ToList();
        for (int i = 0; i < sorted.Count; i++) sorted[i].Position = i + 1;
        return Task.FromResult<IReadOnlyList<LeaderboardEntry>>(sorted);
    }

    public Task<LeaderboardEntry?> GetParticipantRankingAsync(Guid tournamentId, Guid participantId, CancellationToken cancellationToken = default)
    {
        var entries = _rankings.TryGetValue(tournamentId, out var list) ? list : new List<LeaderboardEntry>();
        return Task.FromResult(entries.FirstOrDefault(e => e.ParticipantId == participantId));
    }

    public Task UpdateRankingsAfterMatchAsync(Guid tournamentId, Guid homeParticipantId, Guid awayParticipantId, int homeScore, int awayScore, CancellationToken cancellationToken = default)
    {
        var entries = _rankings.GetOrAdd(tournamentId, _ => new List<LeaderboardEntry>());

        lock (entries)
        {
            var home = entries.FirstOrDefault(e => e.ParticipantId == homeParticipantId);
            var away = entries.FirstOrDefault(e => e.ParticipantId == awayParticipantId);

            if (home == null)
            {
                home = new LeaderboardEntry { ParticipantId = homeParticipantId, ParticipantName = "Participant" };
                entries.Add(home);
            }
            if (away == null)
            {
                away = new LeaderboardEntry { ParticipantId = awayParticipantId, ParticipantName = "Participant" };
                entries.Add(away);
            }

            home.MatchesPlayed++;
            away.MatchesPlayed++;
            home.GoalsFor += homeScore;
            home.GoalsAgainst += awayScore;
            away.GoalsFor += awayScore;
            away.GoalsAgainst += homeScore;
            home.GoalDifference = home.GoalsFor - home.GoalsAgainst;
            away.GoalDifference = away.GoalsFor - away.GoalsAgainst;

            if (homeScore > awayScore)
            {
                home.Wins++;
                home.Points += 3;
                away.Losses++;
            }
            else if (homeScore < awayScore)
            {
                away.Wins++;
                away.Points += 3;
                home.Losses++;
            }
            else
            {
                home.Draws++;
                away.Draws++;
                home.Points++;
                away.Points++;
            }

            home.WinPercentage = home.MatchesPlayed > 0 ? (decimal)home.Wins / home.MatchesPlayed * 100 : 0;
            away.WinPercentage = away.MatchesPlayed > 0 ? (decimal)away.Wins / away.MatchesPlayed * 100 : 0;

            var sorted = entries.OrderByDescending(e => e.Points).ThenByDescending(e => e.Wins).ThenByDescending(e => e.GoalDifference).ToList();
            for (int i = 0; i < sorted.Count; i++) sorted[i].Position = i + 1;
        }

        _logger.LogInformation("Rankings updated for tournament {TournamentId}", tournamentId);
        return Task.CompletedTask;
    }
}
