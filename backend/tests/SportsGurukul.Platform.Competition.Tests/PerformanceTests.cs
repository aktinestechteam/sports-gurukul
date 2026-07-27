using SportsGurukul.Platform.Competition.Engines;
using SportsGurukul.Platform.Competition.Engines.Formats;
using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;
using SportsGurukul.Platform.Competition.Seeding;
using SportsGurukul.Platform.Competition.Services;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace SportsGurukul.Platform.Competition.Tests;

public class PerformanceTests
{
    private readonly ITestOutputHelper _output;

    public PerformanceTests(ITestOutputHelper output)
    {
        _output = output;
    }

    private CompetitionEngine CreateEngine()
    {
        var formatStrategies = new List<IFormatStrategy>
        {
            new SingleEliminationStrategy(), new DoubleEliminationStrategy(),
            new RoundRobinStrategy(), new SwissSystemStrategy(), new LeagueStrategy(),
            new HybridTournamentStrategy(), new GroupStageKnockoutStrategy()
        };
        var seedingStrategies = new List<ISeedingStrategy>
        {
            new RandomSeedingStrategy(), new RankingBasedSeedingStrategy(),
            new ManualSeedingStrategy(), new RegionalSeedingStrategy(),
            new AcademyBasedSeedingStrategy(), new BalancedDrawSeedingStrategy()
        };

        return new CompetitionEngine(
            new BracketGenerationService(formatStrategies, NullLogger<BracketGenerationService>.Instance),
            new FixtureGenerationService(NullLogger<FixtureGenerationService>.Instance),
            new SeedingService(seedingStrategies, NullLogger<SeedingService>.Instance),
            new AdvancementService(NullLogger<AdvancementService>.Instance),
            new RankingCalculator(NullLogger<RankingCalculator>.Instance),
            NullLogger<CompetitionEngine>.Instance);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(1000)]
    public async Task SingleElimination_Performance_LessThan500ms(int participantCount)
    {
        var engine = CreateEngine();
        var config = TestHelpers.CreateConfig(CompetitionFormat.SingleElimination);
        var participants = TestHelpers.CreateParticipants(participantCount);

        var sw = Stopwatch.StartNew();
        var result = await engine.GenerateCompetitionAsync(config, participants);
        sw.Stop();

        _output.WriteLine($"Single Elimination ({participantCount} participants): {sw.ElapsedMilliseconds}ms, {result.Matches.Count} matches");
        Assert.True(sw.ElapsedMilliseconds < 500, $"Took {sw.ElapsedMilliseconds}ms, expected <500ms");
    }

    [Fact]
    public async Task SingleElimination_LargeScale_GeneratesCorrectly()
    {
        var engine = CreateEngine();
        var config = TestHelpers.CreateConfig(CompetitionFormat.SingleElimination);
        var participants = TestHelpers.CreateParticipants(10000);

        var result = await engine.GenerateCompetitionAsync(config, participants);

        Assert.NotNull(result);
        Assert.NotEmpty(result.Matches);
    }

    [Theory]
    [InlineData(100)]
    public async Task RoundRobin_Performance_LessThan500ms(int participantCount)
    {
        var engine = CreateEngine();
        var config = TestHelpers.CreateConfig(CompetitionFormat.RoundRobin);
        var participants = TestHelpers.CreateParticipants(participantCount);

        var sw = Stopwatch.StartNew();
        var result = await engine.GenerateCompetitionAsync(config, participants);
        sw.Stop();

        _output.WriteLine($"Round Robin ({participantCount} participants): {sw.ElapsedMilliseconds}ms, {result.Matches.Count} matches");
        Assert.True(sw.ElapsedMilliseconds < 500, $"Took {sw.ElapsedMilliseconds}ms, expected <500ms");
    }

    [Theory]
    [InlineData(100)]
    [InlineData(1000)]
    public async Task RankingCalculation_Performance_LessThan300ms(int participantCount)
    {
        var engine = CreateEngine();
        var config = TestHelpers.CreateConfig(CompetitionFormat.RoundRobin);
        var participants = TestHelpers.CreateParticipants(participantCount);
        var pIds = participants.Select(p => p.Id).ToList();

        var matches = Enumerable.Range(0, participantCount)
            .SelectMany(i => Enumerable.Range(i + 1, Math.Min(5, participantCount - i - 1))
                .Select(j => TestHelpers.CreateCompletedMatch(pIds[i], pIds[j])))
            .ToList();

        var sw = Stopwatch.StartNew();
        var result = await engine.CalculateRankingsAsync(config, matches, participants);
        sw.Stop();

        _output.WriteLine($"Ranking ({participantCount} participants, {matches.Count} matches): {sw.ElapsedMilliseconds}ms");
        Assert.True(sw.ElapsedMilliseconds < 300, $"Took {sw.ElapsedMilliseconds}ms, expected <300ms");
    }
}
