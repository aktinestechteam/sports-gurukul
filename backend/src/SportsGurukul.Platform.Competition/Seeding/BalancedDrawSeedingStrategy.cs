using SportsGurukul.Platform.Competition.Models;

namespace SportsGurukul.Platform.Competition.Seeding;

public class BalancedDrawSeedingStrategy : ISeedingStrategy
{
    public string StrategyName => "BalancedDraw";

    public IReadOnlyList<Seed> GenerateSeeds(IReadOnlyList<Participant> participants, Guid tournamentId)
    {
        var sorted = participants
            .OrderBy(p => p.Ranking ?? int.MaxValue)
            .ThenBy(p => p.Name)
            .ToList();

        var result = new Seed[sorted.Count];
        int position = 0;
        int left = 0;
        int right = sorted.Count - 1;

        for (int i = 0; i < sorted.Count; i++)
        {
            var participant = sorted[i];
            int targetPosition = i % 2 == 0 ? left++ : right--;

            result[targetPosition] = new Seed
            {
                Id = Guid.NewGuid(),
                TournamentId = tournamentId,
                Position = targetPosition + 1,
                ParticipantId = participant.Id,
                ParticipantName = participant.Name,
                SeedNumber = (targetPosition + 1).ToString(),
                Region = participant.Region,
                AcademyId = participant.AcademyId,
                CurrentRanking = participant.Ranking
            };
        }

        return result.ToList();
    }
}
