using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.Competition.Interfaces;
using SportsGurukul.Platform.Competition.Models;

namespace SportsGurukul.Platform.Competition.Services;

public class FixtureGenerationService : IFixtureGenerationService
{
    private readonly ILogger<FixtureGenerationService> _logger;

    public FixtureGenerationService(ILogger<FixtureGenerationService> logger)
    {
        _logger = logger;
    }

    public Task<IReadOnlyList<Fixture>> GenerateFixturesAsync(
        IReadOnlyList<CompetitionMatch> matches,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating fixtures for {MatchCount} matches", matches.Count);

        var fixtures = matches.Select((m, i) => new Fixture
        {
            Id = Guid.NewGuid(),
            FixtureNumber = i + 1,
            TournamentId = Guid.Empty,
            ScheduledDate = m.ScheduledDate,
            ScheduledTime = m.ScheduledTime,
            VenueId = m.VenueId,
            CourtId = m.CourtId,
            HomeTeamName = m.HomeParticipantName,
            AwayTeamName = m.AwayParticipantName,
            HomeParticipantId = m.HomeParticipantId,
            AwayParticipantId = m.AwayParticipantId,
            IsPublished = false
        }).ToList();

        return Task.FromResult<IReadOnlyList<Fixture>>(fixtures);
    }
}
