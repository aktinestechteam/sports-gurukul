using SportsGurukul.Platform.Competition.Models;

namespace SportsGurukul.Platform.Competition.Seeding;

public class AcademyBasedSeedingStrategy : ISeedingStrategy
{
    public string StrategyName => "AcademyBased";

    public IReadOnlyList<Seed> GenerateSeeds(IReadOnlyList<Participant> participants, Guid tournamentId)
    {
        var grouped = participants
            .GroupBy(p => p.AcademyId ?? Guid.NewGuid())
            .OrderBy(g => g.Key)
            .ToList();

        var result = new List<Seed>();
        int position = 1;
        int round = 0;

        while (result.Count < participants.Count)
        {
            var group = grouped[round % grouped.Count];
            var remaining = group.Where(p => result.All(r => r.ParticipantId != p.Id)).ToList();

            if (remaining.Count > 0)
            {
                var participant = remaining.First();
                result.Add(new Seed
                {
                    Id = Guid.NewGuid(),
                    TournamentId = tournamentId,
                    Position = position++,
                    ParticipantId = participant.Id,
                    ParticipantName = participant.Name,
                    SeedNumber = position.ToString(),
                    Region = participant.Region,
                    AcademyId = participant.AcademyId,
                    CurrentRanking = participant.Ranking
                });
            }

            round++;
        }

        return result;
    }
}
