using SportsGurukul.Platform.Competition.Models;

namespace SportsGurukul.Platform.Competition.Interfaces;

public interface IFixtureGenerationService
{
    Task<IReadOnlyList<Fixture>> GenerateFixturesAsync(
        IReadOnlyList<CompetitionMatch> matches,
        CancellationToken cancellationToken = default);
}
