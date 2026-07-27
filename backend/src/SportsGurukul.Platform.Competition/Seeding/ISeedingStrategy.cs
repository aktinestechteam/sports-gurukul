using SportsGurukul.Platform.Competition.Models;

namespace SportsGurukul.Platform.Competition.Seeding;

public interface ISeedingStrategy
{
    string StrategyName { get; }
    IReadOnlyList<Seed> GenerateSeeds(IReadOnlyList<Participant> participants, Guid tournamentId);
}
