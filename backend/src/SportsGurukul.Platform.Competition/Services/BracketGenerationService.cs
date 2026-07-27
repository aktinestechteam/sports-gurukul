using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.Competition.Engines.Formats;
using SportsGurukul.Platform.Competition.Interfaces;
using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Services;

public class BracketGenerationService : IBracketGenerationService
{
    private readonly IReadOnlyDictionary<CompetitionFormat, IFormatStrategy> _strategies;
    private readonly ILogger<BracketGenerationService> _logger;

    public BracketGenerationService(IEnumerable<IFormatStrategy> strategies, ILogger<BracketGenerationService> logger)
    {
        _strategies = strategies.ToDictionary(s => s.Format);
        _logger = logger;
    }

    public async Task<IReadOnlyList<Bracket>> GenerateBracketsAsync(
        CompetitionConfig config,
        IReadOnlyList<Participant> participants,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating brackets for format: {Format}", config.Format);

        if (!_strategies.TryGetValue(config.Format, out var strategy))
            throw new NotSupportedException($"Competition format '{config.Format}' is not supported.");

        var matches = await strategy.GenerateMatchesAsync(participants, config, cancellationToken);

        var bracket = new Bracket
        {
            Id = Guid.NewGuid(),
            Name = $"{config.Format} Bracket",
            Type = BracketType.Main,
            Format = config.Format,
            Matches = matches.ToList(),
            Rounds = matches
                .GroupBy(m => m.RoundNumber)
                .Select(g => new BracketRound
                {
                    RoundNumber = g.Key,
                    RoundName = $"Round {g.Key}",
                    Matches = g.ToList()
                })
                .ToList()
        };

        return new List<Bracket> { bracket };
    }
}
