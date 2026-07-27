using SportsGurukul.Platform.Competition.Models;

namespace SportsGurukul.Platform.Competition.Interfaces;

public interface IAdvancementService
{
    Task<IReadOnlyList<CompetitionMatch>> AdvanceWinnerAsync(
        CompetitionMatch completedMatch,
        IReadOnlyList<CompetitionMatch> allMatches,
        CancellationToken cancellationToken = default);
}
