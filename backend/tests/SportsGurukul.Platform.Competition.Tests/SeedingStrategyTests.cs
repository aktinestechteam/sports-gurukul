using SportsGurukul.Platform.Competition.Seeding;
using Xunit;

namespace SportsGurukul.Platform.Competition.Tests;

public class SeedingStrategyTests
{
    [Fact]
    public void RandomSeeding_GeneratesCorrectCount()
    {
        var strategy = new RandomSeedingStrategy();
        var participants = TestHelpers.CreateParticipants(8);

        var seeds = strategy.GenerateSeeds(participants, Guid.NewGuid());

        Assert.Equal(8, seeds.Count);
    }

    [Fact]
    public void RankingBasedSeeding_SortsByRanking()
    {
        var strategy = new RankingBasedSeedingStrategy();
        var participants = TestHelpers.CreateParticipants(8);

        var seeds = strategy.GenerateSeeds(participants, Guid.NewGuid());

        Assert.Equal(1, seeds[0].Position);
        for (int i = 1; i < seeds.Count; i++)
        {
            Assert.True(seeds[i].Position > seeds[i - 1].Position);
        }
    }

    [Fact]
    public void ManualSeeding_PreservesExistingOrder()
    {
        var strategy = new ManualSeedingStrategy();
        var participants = TestHelpers.CreateParticipants(8);

        var seeds = strategy.GenerateSeeds(participants, Guid.NewGuid());

        Assert.Equal(8, seeds.Count);
        Assert.All(seeds, s => Assert.False(string.IsNullOrEmpty(s.SeedNumber)));
    }

    [Fact]
    public void RegionalSeeding_DistributesByRegion()
    {
        var strategy = new RegionalSeedingStrategy();
        var participants = TestHelpers.CreateParticipants(12);

        var seeds = strategy.GenerateSeeds(participants, Guid.NewGuid());

        Assert.Equal(12, seeds.Count);
        var regions = seeds.Select(s => s.Region).Distinct().ToList();
        Assert.True(regions.Count > 1);
    }

    [Fact]
    public void AcademyBasedSeeding_InterleavesAcademies()
    {
        var strategy = new AcademyBasedSeedingStrategy();
        var participants = TestHelpers.CreateParticipants(12);

        var seeds = strategy.GenerateSeeds(participants, Guid.NewGuid());

        Assert.Equal(12, seeds.Count);
    }

    [Fact]
    public void BalancedDrawSeeding_ProducesValidBracket()
    {
        var strategy = new BalancedDrawSeedingStrategy();
        var participants = TestHelpers.CreateParticipants(8);

        var seeds = strategy.GenerateSeeds(participants, Guid.NewGuid());

        Assert.Equal(8, seeds.Count);
        Assert.Equal(1, seeds[0].Position);
        Assert.Equal(8, seeds[^1].Position);
    }

    [Theory]
    [InlineData(4)]
    [InlineData(7)]
    [InlineData(16)]
    [InlineData(32)]
    public void AllStrategies_HandleVariousParticipantCounts(int count)
    {
        var strategies = new ISeedingStrategy[]
        {
            new RandomSeedingStrategy(),
            new RankingBasedSeedingStrategy(),
            new ManualSeedingStrategy(),
            new BalancedDrawSeedingStrategy()
        };

        var participants = TestHelpers.CreateParticipants(count);

        foreach (var strategy in strategies)
        {
            var seeds = strategy.GenerateSeeds(participants, Guid.NewGuid());
            Assert.Equal(count, seeds.Count);
        }
    }
}
