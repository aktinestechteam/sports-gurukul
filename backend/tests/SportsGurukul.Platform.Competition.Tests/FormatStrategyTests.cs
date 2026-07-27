using SportsGurukul.Platform.Competition.Engines.Formats;
using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;
using Xunit;

namespace SportsGurukul.Platform.Competition.Tests;

public class FormatStrategyTests
{
    [Fact]
    public async Task SingleElimination_GeneratesCorrectNumberOfMatches()
    {
        var strategy = new SingleEliminationStrategy();
        var participants = TestHelpers.CreateParticipants(8);
        var config = TestHelpers.CreateConfig(CompetitionFormat.SingleElimination);

        var matches = await strategy.GenerateMatchesAsync(participants, config);

        Assert.Equal(7, matches.Count);
    }

    [Theory]
    [InlineData(4, 3)]
    [InlineData(8, 7)]
    [InlineData(16, 15)]
    [InlineData(32, 31)]
    public async Task SingleElimination_MatchesIsPowerOf2Minus1(int participantCount, int expectedMatches)
    {
        var strategy = new SingleEliminationStrategy();
        var participants = TestHelpers.CreateParticipants(participantCount);
        var config = TestHelpers.CreateConfig();

        var matches = await strategy.GenerateMatchesAsync(participants, config);

        Assert.Equal(expectedMatches, matches.Count);
    }

    [Fact]
    public async Task RoundRobin_GeneratesAllPairings()
    {
        var strategy = new RoundRobinStrategy();
        var participants = TestHelpers.CreateParticipants(6);
        var config = TestHelpers.CreateConfig(CompetitionFormat.RoundRobin);

        var matches = await strategy.GenerateMatchesAsync(participants, config);

        int expectedMatches = 6 * 5 / 2;
        Assert.Equal(expectedMatches, matches.Count);
    }

    [Fact]
    public async Task DoubleElimination_GeneratesMainAndLosersBrackets()
    {
        var strategy = new DoubleEliminationStrategy();
        var participants = TestHelpers.CreateParticipants(8);
        var config = TestHelpers.CreateConfig(CompetitionFormat.DoubleElimination);

        var matches = await strategy.GenerateMatchesAsync(participants, config);

        Assert.True(matches.Count > 7);
        var winners = matches.Where(m => m.BracketType == BracketType.Winners || m.BracketType == BracketType.Main);
        var losers = matches.Where(m => m.BracketType == BracketType.Losers);
        Assert.True(winners.Any());
        Assert.True(losers.Any());
    }

    [Fact]
    public async Task SwissSystem_GeneratesConfiguredRounds()
    {
        var strategy = new SwissSystemStrategy();
        var participants = TestHelpers.CreateParticipants(8);
        var config = TestHelpers.CreateConfig(CompetitionFormat.SwissSystem);
        config.RoundsCount = 4;

        var matches = await strategy.GenerateMatchesAsync(participants, config);

        Assert.True(matches.Count > 0);
        int expectedPerRound = 8 / 2;
        Assert.Equal(expectedPerRound * 4, matches.Count);
    }

    [Fact]
    public async Task League_GeneratesDivisionMatches()
    {
        var strategy = new LeagueStrategy();
        var participants = TestHelpers.CreateParticipants(12);
        var config = TestHelpers.CreateConfig(CompetitionFormat.League);
        config.GroupsCount = 3;

        var matches = await strategy.GenerateMatchesAsync(participants, config);

        Assert.True(matches.Count > 0);
    }

    [Fact]
    public async Task GroupStageKnockout_GeneratesGroupAndKnockoutMatches()
    {
        var strategy = new GroupStageKnockoutStrategy();
        var participants = TestHelpers.CreateParticipants(16);
        var config = TestHelpers.CreateConfig(CompetitionFormat.GroupStageKnockout);
        config.GroupsCount = 4;
        config.AdvancementPerGroup = 2;

        var matches = await strategy.GenerateMatchesAsync(participants, config);

        Assert.True(matches.Count > 0);
    }

    [Fact]
    public async Task SingleElimination_HandlesOddNumberOfParticipants()
    {
        var strategy = new SingleEliminationStrategy();
        var participants = TestHelpers.CreateParticipants(7);
        var config = TestHelpers.CreateConfig();

        var matches = await strategy.GenerateMatchesAsync(participants, config);

        Assert.True(matches.Count >= 3);
    }

    [Fact]
    public async Task RoundRobin_HandlesTwoParticipants()
    {
        var strategy = new RoundRobinStrategy();
        var participants = TestHelpers.CreateParticipants(2);
        var config = TestHelpers.CreateConfig(CompetitionFormat.RoundRobin);

        var matches = await strategy.GenerateMatchesAsync(participants, config);

        Assert.Single(matches);
    }

    [Fact]
    public async Task AllFormats_HandleSingleParticipant()
    {
        var strategies = new IFormatStrategy[]
        {
            new SingleEliminationStrategy(),
            new RoundRobinStrategy(),
            new SwissSystemStrategy()
        };

        var participants = TestHelpers.CreateParticipants(1);
        var config = TestHelpers.CreateConfig();

        foreach (var strategy in strategies)
        {
            var matches = await strategy.GenerateMatchesAsync(participants, config);
            Assert.NotNull(matches);
        }
    }
}
