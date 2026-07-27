using SportsGurukul.Platform.Competition.Models;

namespace SportsGurukul.Platform.Competition.Seeding;

public class RegionalSeedingStrategy : ISeedingStrategy
{
    public string StrategyName => "Regional";

    public IReadOnlyList<Seed> GenerateSeeds(IReadOnlyList<Participant> participants, Guid tournamentId)
    {
        var grouped = participants.GroupBy(p => p.Region ?? "Unknown").ToList();
        var result = new List<Seed>();
        int position = 1;
        int round = 0;
        bool ascending = true;

        while (result.Count < participants.Count)
        {
            var groupIndex = ascending ? round % grouped.Count : (grouped.Count - 1) - (round % grouped.Count);
            var group = grouped[groupIndex];
            var participant = group.ElementAtOrDefault(result.Count(p => p.Region == group.Key));

            if (participant != null)
            {
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
            if (round % grouped.Count == 0) ascending = !ascending;
        }

        return result;
    }
}
