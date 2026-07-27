using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.Competition.Interfaces;
using SportsGurukul.Platform.Competition.Models;

namespace SportsGurukul.Platform.Competition.Engines;

public class CompetitionEngine : ICompetitionEngine
{
    private readonly IBracketGenerationService _bracketService;
    private readonly IFixtureGenerationService _fixtureService;
    private readonly ISeedingService _seedingService;
    private readonly IAdvancementService _advancementService;
    private readonly IRankingCalculator _rankingCalculator;
    private readonly ILogger<CompetitionEngine> _logger;

    public CompetitionEngine(
        IBracketGenerationService bracketService,
        IFixtureGenerationService fixtureService,
        ISeedingService seedingService,
        IAdvancementService advancementService,
        IRankingCalculator rankingCalculator,
        ILogger<CompetitionEngine> logger)
    {
        _bracketService = bracketService;
        _fixtureService = fixtureService;
        _seedingService = seedingService;
        _advancementService = advancementService;
        _rankingCalculator = rankingCalculator;
        _logger = logger;
    }

    public async Task<CompetitionResult> GenerateCompetitionAsync(
        CompetitionConfig config,
        IReadOnlyList<Participant> participants,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Generating competition: Format={Format}, Participants={Count}",
            config.Format, participants.Count);

        var seeds = await _seedingService.GenerateSeedsAsync(config, participants, cancellationToken);

        var seededParticipants = seeds.Select(s => new Participant
        {
            Id = s.ParticipantId,
            Name = s.ParticipantName ?? "Unknown",
            Ranking = s.CurrentRanking,
            Region = s.Region,
            AcademyId = s.AcademyId,
            SeedNumber = s.SeedNumber
        }).ToList();

        var brackets = await _bracketService.GenerateBracketsAsync(config, seededParticipants, cancellationToken);

        var allMatches = brackets.SelectMany(b => b.Matches).ToList();

        var fixtures = await _fixtureService.GenerateFixturesAsync(allMatches, cancellationToken);

        return new CompetitionResult
        {
            Brackets = brackets.ToList(),
            Fixtures = fixtures.ToList(),
            Seeds = seeds.ToList(),
            Matches = allMatches,
            Format = config.Format
        };
    }

    public Task<IReadOnlyList<CompetitionMatch>> AdvanceWinnerAsync(
        CompetitionMatch completedMatch,
        IReadOnlyList<CompetitionMatch> allMatches,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Advancing winner from match: {MatchId}", completedMatch.Id);
        return _advancementService.AdvanceWinnerAsync(completedMatch, allMatches, cancellationToken);
    }

    public async Task<RankingResult> CalculateRankingsAsync(
        CompetitionConfig config,
        IReadOnlyList<CompetitionMatch> completedMatches,
        IReadOnlyList<Participant> participants,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calculating rankings for {ParticipantCount} participants", participants.Count);

        var rankings = await _rankingCalculator.CalculateRankingsAsync(
            config, completedMatches, participants, cancellationToken);

        var medalStandings = rankings
            .Where(r => r.Rank <= 3)
            .OrderBy(r => r.Rank)
            .ToList();

        return new RankingResult
        {
            Rankings = rankings.ToList(),
            MedalStandings = medalStandings
        };
    }
}
