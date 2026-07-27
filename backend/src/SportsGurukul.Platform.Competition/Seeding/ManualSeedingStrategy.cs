using SportsGurukul.Platform.Competition.Models;

namespace SportsGurukul.Platform.Competition.Seeding;

public class ManualSeedingStrategy : ISeedingStrategy
{
    public string StrategyName => "Manual";

    public IReadOnlyList<Seed> GenerateSeeds(IReadOnlyList<Participant> participants, Guid tournamentId)
    {
        var seeded = participants
            .Where(p => p.Ranking.HasValue)
            .OrderBy(p => p.Ranking!.Value)
            .ToList();

        var unseeded = participants
            .Where(p => !p.Ranking.HasValue)
            .ToList();

        var all = seeded.Concat(unseeded).ToList();

        return all.Select((p, i) => new Seed
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
