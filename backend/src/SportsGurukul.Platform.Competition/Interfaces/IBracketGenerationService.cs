using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Interfaces;

public interface IBracketGenerationService
{
    Task<IReadOnlyList<Bracket>> GenerateBracketsAsync(
        CompetitionConfig config,
        IReadOnlyList<Participant> participants,
        CancellationToken cancellationToken = default);
}
