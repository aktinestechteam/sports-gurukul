using SportsGurukul.Platform.Competition.Engines;
using SportsGurukul.Platform.Competition.Engines.Formats;
using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;
using SportsGurukul.Platform.Competition.Seeding;
using SportsGurukul.Platform.Competition.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace SportsGurukul.Platform.Competition.Tests;

public class CompetitionEngineTests
{
    private CompetitionEngine CreateEngine()
    {
        var formatStrategies = new List<IFormatStrategy>
        {
            new SingleEliminationStrategy(),
            new DoubleEliminationStrategy(),
            new RoundRobinStrategy(),
            new SwissSystemStrategy(),
            new LeagueStrategy(),
            new HybridTournamentStrategy(),
            new GroupStageKnockoutStrategy()
        };

        var seedingStrategies = new List<ISeedingStrategy>
        {
            new RandomSeedingStrategy(),
            new RankingBasedSeedingStrategy(),
            new ManualSeedingStrategy(),
            new RegionalSeedingStrategy(),
            new AcademyBasedSeedingStrategy(),
            new BalancedDrawSeedingStrategy()
        };

        var bracketService = new BracketGenerationService(formatStrategies, NullLogger<BracketGenerationService>.Instance);
        var fixtureService = new FixtureGenerationService(NullLogger<FixtureGenerationService>.Instance);
        var seedingService = new SeedingService(seedingStrategies, NullLogger<SeedingService>.Instance);
        var advancementService = new AdvancementService(NullLogger<AdvancementService>.Instance);
        var rankingCalculator = new RankingCalculator(NullLogger<RankingCalculator>.Instance);

        return new CompetitionEngine(bracketService, fixtureService, seedingService, advancementService, rankingCalculator, NullLogger<CompetitionEngine>.Instance);
    }

    [Theory]
    [InlineData(CompetitionFormat.SingleElimination, 8)]
    [InlineData(CompetitionFormat.DoubleElimination, 8)]
    [InlineData(CompetitionFormat.RoundRobin, 6)]
    [InlineData(CompetitionFormat.SwissSystem, 8)]
    [InlineData(CompetitionFormat.GroupStageKnockout, 16)]
    public async Task GenerateCompetition_AllFormats_ProducesValidResult(CompetitionFormat format, int participantCount)
    {
        var engine = CreateEngine();
        var config = TestHelpers.CreateConfig(format);
        config.GroupsCount = 4;
        config.AdvancementPerGroup = 2;
        config.RoundsCount = 4;
        var participants = TestHelpers.CreateParticipants(participantCount);

        var result = await engine.GenerateCompetitionAsync(config, participants);

        Assert.NotNull(result);
        Assert.Equal(format, result.Format);
        Assert.NotEmpty(result.Brackets);
        Assert.NotEmpty(result.Matches);
    }

    [Fact]
    public async Task CalculateRankings_Engine_DelegatesToCalculator()
    {
        var engine = CreateEngine();
        var config = TestHelpers.CreateConfig(CompetitionFormat.RoundRobin);
        var participants = TestHelpers.CreateParticipants(4);
        var pIds = participants.Select(p => p.Id).ToList();

        var matches = new List<CompetitionMatch>
        {
            TestHelpers.CreateCompletedMatch(pIds[0], pIds[1], 2, 1),
        };

        var result = await engine.CalculateRankingsAsync(config, matches, participants);

        Assert.NotEmpty(result.Rankings);
    }
}
