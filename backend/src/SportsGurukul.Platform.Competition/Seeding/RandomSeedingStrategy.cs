using SportsGurukul.Platform.Competition.Models;

namespace SportsGurukul.Platform.Competition.Seeding;

public class RandomSeedingStrategy : ISeedingStrategy
{
    public string StrategyName => "Random";

    public IReadOnlyList<Seed> GenerateSeeds(IReadOnlyList<Participant> participants, Guid tournamentId)
    {
        var shuffled = participants.OrderBy(_ => Random.Shared.Next()).ToList();
        return shuffled.Select((p, i) => new Seed
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
