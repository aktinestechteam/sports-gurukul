using SportsGurukul.Platform.Competition.Models;

namespace SportsGurukul.Platform.Competition.Seeding;

public class RankingBasedSeedingStrategy : ISeedingStrategy
{
    public string StrategyName => "RankingBased";

    public IReadOnlyList<Seed> GenerateSeeds(IReadOnlyList<Participant> participants, Guid tournamentId)
    {
        var sorted = participants
            .OrderBy(p => p.Ranking ?? int.MaxValue)
            .ThenBy(p => p.Name)
            .ToList();

        return sorted.Select((p, i) => new Seed
        {
            Id = Guid.NewGuid(),
            TournamentId = tournamentId,
            Position = i + 1,
            ParticipantId = p.Id,
            ParticipantName = p.Name,
            SeedNumber = (i + 1).ToString(),
            Region = p.Region,
            AcademyId = p.AcademyId,
            CurrentRanking = p.Ranking
        }).ToList();
    }
}
