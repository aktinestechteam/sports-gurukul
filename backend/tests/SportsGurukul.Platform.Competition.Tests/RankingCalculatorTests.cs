using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;
using SportsGurukul.Platform.Competition.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace SportsGurukul.Platform.Competition.Tests;

public class RankingCalculatorTests
{
    private readonly RankingCalculator _calculator = new(NullLogger<RankingCalculator>.Instance);

    [Fact]
    public async Task CalculateRankings_RanksByPoints()
    {
        var config = TestHelpers.CreateConfig(CompetitionFormat.RoundRobin);
        var participants = TestHelpers.CreateParticipants(4);
        var pIds = participants.Select(p => p.Id).ToList();

        var matches = new List<CompetitionMatch>
        {
            TestHelpers.CreateCompletedMatch(pIds[0], pIds[1], 3, 1),
            TestHelpers.CreateCompletedMatch(pIds[2], pIds[3], 2, 2),
            TestHelpers.CreateCompletedMatch(pIds[0], pIds[2], 1, 0),
        };

        var rankings = await _calculator.CalculateRankingsAsync(config, matches, participants);

        Assert.Equal(4, rankings.Count);
        Assert.Equal(1, rankings[0].Rank);
        Assert.True(rankings[0].Points >= rankings[1].Points);
    }

    [Fact]
    public async Task CalculateRankings_HandlesDraw()
    {
        var config = TestHelpers.CreateConfig(CompetitionFormat.RoundRobin);
        var participants = TestHelpers.CreateParticipants(2);
        var pIds = participants.Select(p => p.Id).ToList();

        var matches = new List<CompetitionMatch>
        {
            TestHelpers.CreateCompletedMatch(pIds[0], pIds[1], 1, 1),
        };

        var rankings = await _calculator.CalculateRankingsAsync(config, matches, participants);

        Assert.Equal(2, rankings.Count);
        Assert.All(rankings, r => Assert.Equal(1, r.Draws));
        Assert.All(rankings, r => Assert.Equal(config.PointsForDraw, r.Points));
    }

    [Fact]
    public async Task CalculateRankings_CalculatesGoalDifference()
    {
        var config = TestHelpers.CreateConfig(CompetitionFormat.RoundRobin);
        var participants = TestHelpers.CreateParticipants(3);
        var pIds = participants.Select(p => p.Id).ToList();

        var matches = new List<CompetitionMatch>
        {
            TestHelpers.CreateCompletedMatch(pIds[0], pIds[1], 3, 1),
            TestHelpers.CreateCompletedMatch(pIds[0], pIds[2], 2, 0),
        };

        var rankings = await _calculator.CalculateRankingsAsync(config, matches, participants);

        var topScorer = rankings.First(r => r.ParticipantId == pIds[0]);
        Assert.Equal(5, topScorer.GoalsFor);
        Assert.Equal(1, topScorer.GoalsAgainst);
        Assert.Equal(4, topScorer.GoalDifference);
    }

    [Fact]
    public async Task CalculateRankings_EliminationFormat()
    {
        var config = TestHelpers.CreateConfig(CompetitionFormat.SingleElimination);
        var participants = TestHelpers.CreateParticipants(8);
        var pIds = participants.Select(p => p.Id).ToList();

        var matches = new List<CompetitionMatch>
        {
            TestHelpers.CreateCompletedMatch(pIds[0], pIds[1]),
            TestHelpers.CreateCompletedMatch(pIds[2], pIds[3]),
            TestHelpers.CreateCompletedMatch(pIds[4], pIds[5]),
            TestHelpers.CreateCompletedMatch(pIds[6], pIds[7]),
        };

        var rankings = await _calculator.CalculateRankingsAsync(config, matches, participants);

        Assert.Equal(8, rankings.Count);
        var winners = rankings.Where(r => r.Wins > 0).ToList();
        Assert.Equal(4, winners.Count);
        Assert.All(winners, r => Assert.Equal(1, r.Wins));
        var losers = rankings.Where(r => r.Losses > 0).ToList();
        Assert.Equal(4, losers.Count);
    }
}
