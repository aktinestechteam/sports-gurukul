using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Interfaces;

public interface ISeedingService
{
    Task<IReadOnlyList<Seed>> GenerateSeedsAsync(
        CompetitionConfig config,
        IReadOnlyList<Participant> participants,
        CancellationToken cancellationToken = default);
}
