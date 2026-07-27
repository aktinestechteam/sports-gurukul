using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.Competition.Interfaces;
using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;
using SportsGurukul.Platform.Competition.Seeding;

namespace SportsGurukul.Platform.Competition.Services;

public class SeedingService : ISeedingService
{
    private readonly IReadOnlyDictionary<string, ISeedingStrategy> _strategies;
    private readonly ILogger<SeedingService> _logger;

    public SeedingService(IEnumerable<ISeedingStrategy> strategies, ILogger<SeedingService> logger)
    {
        _strategies = strategies.ToDictionary(s => s.StrategyName);
        _logger = logger;
    }

    public Task<IReadOnlyList<Seed>> GenerateSeedsAsync(
        CompetitionConfig config,
        IReadOnlyList<Participant> participants,
        CancellationToken cancellationToken = default)
    {
        var strategyName = config.SeedingStrategy.ToString();
        _logger.LogInformation("Generating seeds using strategy: {Strategy}", strategyName);

        if (!_strategies.TryGetValue(strategyName, out var strategy))
        {
            _logger.LogWarning("Seeding strategy '{Strategy}' not found, falling back to Random", strategyName);
            strategy = _strategies["Random"];
        }

        var seeds = strategy.GenerateSeeds(participants, config.TournamentId);
        return Task.FromResult(seeds);
    }
}
