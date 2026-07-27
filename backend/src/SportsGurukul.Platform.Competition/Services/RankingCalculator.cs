using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.Competition.Interfaces;
using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Services;

public class RankingCalculator : IRankingCalculator
{
    private readonly ILogger<RankingCalculator> _logger;

    public RankingCalculator(ILogger<RankingCalculator> logger)
    {
        _logger = logger;
    }

    public Task<IReadOnlyList<Ranking>> CalculateRankingsAsync(
        CompetitionConfig config,
        IReadOnlyList<CompetitionMatch> completedMatches,
        IReadOnlyList<Participant> participants,
        CancellationToken cancellationToken = default)
    {
        return config.Format switch
        {
            CompetitionFormat.RoundRobin or CompetitionFormat.League or CompetitionFormat.SwissSystem
                => CalculateRoundRobinRankingsAsync(completedMatches, participants, config, cancellationToken),
            CompetitionFormat.SingleElimination or CompetitionFormat.DoubleElimination or CompetitionFormat.HybridTournament or CompetitionFormat.GroupStageKnockout
                => CalculateEliminationRankingsAsync(completedMatches, participants, config, cancellationToken),
            _ => CalculateRoundRobinRankingsAsync(completedMatches, participants, config, cancellationToken)
        };
    }

    public Task<IReadOnlyList<Ranking>> CalculateRoundRobinRankingsAsync(
        IReadOnlyList<CompetitionMatch> matches,
        IReadOnlyList<Participant> participants,
        CompetitionConfig config,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calculating round-robin rankings for {ParticipantCount} participants", participants.Count);

        var completedMatches = matches.Where(m => m.IsCompleted && !m.IsBye).ToList();
        var rankings = participants.Select(p => new Ranking
        {
            Id = Guid.NewGuid(),
            TournamentId = config.TournamentId,
            ParticipantId = p.Id,
            ParticipantName = p.Name
        }).ToList();

        foreach (var match in completedMatches)
        {
            var home = rankings.FirstOrDefault(r => r.ParticipantId == match.HomeParticipantId);
            var away = rankings.FirstOrDefault(r => r.ParticipantId == match.AwayParticipantId);

            if (home is null || away is null) continue;

            home.MatchesPlayed++;
            away.MatchesPlayed++;

            if (match.HomeScore > match.AwayScore)
            {
                home.Wins++;
                home.Points += config.PointsForWin;
                away.Losses++;
                away.Points += config.PointsForLoss;
            }
            else if (match.HomeScore < match.AwayScore)
            {
                away.Wins++;
                away.Points += config.PointsForWin;
                home.Losses++;
                home.Points += config.PointsForLoss;
            }
            else
            {
                home.Draws++;
                away.Draws++;
                home.Points += config.PointsForDraw;
                away.Points += config.PointsForDraw;
            }

            home.GoalsFor += match.HomeScore ?? 0;
            home.GoalsAgainst += match.AwayScore ?? 0;
            away.GoalsFor += match.AwayScore ?? 0;
            away.GoalsAgainst += match.HomeScore ?? 0;

            foreach (var set in match.Sets)
            {
                home.SetsWon += (set.HomeScore > set.AwayScore) ? 1 : 0;
                home.SetsLost += (set.HomeScore < set.AwayScore) ? 1 : 0;
                away.SetsWon += (set.AwayScore > set.HomeScore) ? 1 : 0;
                away.SetsLost += (set.AwayScore < set.HomeScore) ? 1 : 0;
                home.GamesWon += set.HomeScore ?? 0;
                home.GamesLost += set.AwayScore ?? 0;
                away.GamesWon += set.AwayScore ?? 0;
                away.GamesLost += set.HomeScore ?? 0;
            }
        }

        var sorted = ApplyTiebreakers(rankings, config.Tiebreakers);
        for (int i = 0; i < sorted.Count; i++)
            sorted[i].Rank = i + 1;

        return Task.FromResult<IReadOnlyList<Ranking>>(sorted);
    }

    public Task<IReadOnlyList<Ranking>> CalculateLeagueRankingsAsync(
        IReadOnlyList<CompetitionMatch> matches,
        IReadOnlyList<Participant> participants,
        CompetitionConfig config,
        CancellationToken cancellationToken = default)
    {
        return CalculateRoundRobinRankingsAsync(matches, participants, config, cancellationToken);
    }

    private Task<IReadOnlyList<Ranking>> CalculateEliminationRankingsAsync(
        IReadOnlyList<CompetitionMatch> matches,
        IReadOnlyList<Participant> participants,
        CompetitionConfig config,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calculating elimination rankings");

        var completedMatches = matches.Where(m => m.IsCompleted && !m.IsBye).ToList();
        var rankings = participants.Select(p => new Ranking
        {
            Id = Guid.NewGuid(),
            TournamentId = config.TournamentId,
            ParticipantId = p.Id,
            ParticipantName = p.Name
        }).ToList();

        foreach (var match in completedMatches)
        {
            if (match.WinnerId is null) continue;

            var loserId = match.HomeParticipantId == match.WinnerId
                ? match.AwayParticipantId
                : match.HomeParticipantId;

            var winner = rankings.FirstOrDefault(r => r.ParticipantId == match.WinnerId);
            if (winner is not null)
            {
                winner.MatchesPlayed++;
                winner.Wins++;
                winner.Points += config.PointsForWin;
            }

            if (loserId.HasValue)
            {
                var loser = rankings.FirstOrDefault(r => r.ParticipantId == loserId.Value);
                if (loser is not null)
                {
                    loser.MatchesPlayed++;
                    loser.Losses++;
                }
            }
        }

        var maxRound = completedMatches.Any() ? completedMatches.Max(m => m.RoundNumber) : 0;

        foreach (var match in completedMatches.Where(m => m.RoundNumber == maxRound))
        {
            if (match.WinnerId.HasValue)
            {
                var winner = rankings.FirstOrDefault(r => r.ParticipantId == match.WinnerId.Value);
                if (winner is not null) winner.Points += 10;
            }
        }

        var sorted = rankings.OrderByDescending(r => r.Points).ThenByDescending(r => r.Wins).ToList();
        for (int i = 0; i < sorted.Count; i++)
            sorted[i].Rank = i + 1;

        return Task.FromResult<IReadOnlyList<Ranking>>(sorted);
    }

    private static List<Ranking> ApplyTiebreakers(List<Ranking> rankings, List<RankingTiebreaker> tiebreakers)
    {
        IOrderedEnumerable<Ranking> ordered = rankings.OrderByDescending(r => r.Points);

        foreach (var tiebreaker in tiebreakers)
        {
            ordered = tiebreaker switch
            {
                RankingTiebreaker.GoalDifference => ordered.ThenByDescending(r => r.GoalDifference),
                RankingTiebreaker.GoalsScored => ordered.ThenByDescending(r => r.GoalsFor),
                RankingTiebreaker.Wins => ordered.ThenByDescending(r => r.Wins),
                RankingTiebreaker.SetsWon => ordered.ThenByDescending(r => r.SetsWon),
                RankingTiebreaker.GamesWon => ordered.ThenByDescending(r => r.GamesWon),
                _ => ordered
            };
        }

        return ordered.ToList();
    }
}
