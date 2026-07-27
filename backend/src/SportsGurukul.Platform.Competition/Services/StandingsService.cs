using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.Competition.Interfaces;
using SportsGurukul.Platform.Competition.Models;

namespace SportsGurukul.Platform.Competition.Services;

public class StandingsService : IStandingsService
{
    private readonly ConcurrentDictionary<Guid, List<StandingsEntry>> _standings = new();
    private readonly ILogger<StandingsService> _logger;

    public StandingsService(ILogger<StandingsService> logger)
    {
        _logger = logger;
    }

    public Task<IReadOnlyList<StandingsEntry>> GetTournamentStandingsAsync(Guid tournamentId, string? sportCode, CancellationToken cancellationToken = default)
    {
        var entries = _standings.TryGetValue(tournamentId, out var list) ? list : new List<StandingsEntry>();
        var sorted = entries.OrderByDescending(e => e.Points).ThenByDescending(e => e.GoalDifference).ThenByDescending(e => e.GoalsFor).ToList();
        for (int i = 0; i < sorted.Count; i++) sorted[i].Position = i + 1;
        return Task.FromResult<IReadOnlyList<StandingsEntry>>(sorted);
    }

    public Task<StandingsEntry?> GetParticipantStandingAsync(Guid tournamentId, Guid participantId, CancellationToken cancellationToken = default)
    {
        var entries = _standings.TryGetValue(tournamentId, out var list) ? list : new List<StandingsEntry>();
        return Task.FromResult(entries.FirstOrDefault(e => e.ParticipantId == participantId));
    }

    public Task UpdateStandingsAfterMatchAsync(Guid tournamentId, Guid homeParticipantId, Guid awayParticipantId, int homeScore, int awayScore, CancellationToken cancellationToken = default)
    {
        var entries = _standings.GetOrAdd(tournamentId, _ => new List<StandingsEntry>());

        lock (entries)
        {
            var home = entries.FirstOrDefault(e => e.ParticipantId == homeParticipantId);
            var away = entries.FirstOrDefault(e => e.ParticipantId == awayParticipantId);

            if (home == null)
            {
                home = new StandingsEntry { ParticipantId = homeParticipantId, ParticipantName = "Participant" };
                entries.Add(home);
            }
            if (away == null)
            {
                away = new StandingsEntry { ParticipantId = awayParticipantId, ParticipantName = "Participant" };
                entries.Add(away);
            }

            home.Played++;
            away.Played++;
            home.GoalsFor += homeScore;
            home.GoalsAgainst += awayScore;
            away.GoalsFor += awayScore;
            away.GoalsAgainst += homeScore;
            home.GoalDifference = home.GoalsFor - home.GoalsAgainst;
            away.GoalDifference = away.GoalsFor - away.GoalsAgainst;
            home.AverageGoalsPerMatch = home.Played > 0 ? (decimal)home.GoalsFor / home.Played : 0;
            away.AverageGoalsPerMatch = away.Played > 0 ? (decimal)away.GoalsFor / away.Played : 0;

            if (homeScore > awayScore)
            {
                home.Won++;
                home.Points += 3;
                away.Lost++;
            }
            else if (homeScore < awayScore)
            {
                away.Won++;
                away.Points += 3;
                home.Lost++;
            }
            else
            {
                home.Drawn++;
                away.Drawn++;
                home.Points++;
                away.Points++;
            }
        }

        _logger.LogInformation("Standings updated for tournament {TournamentId}", tournamentId);
        return Task.CompletedTask;
    }
}
