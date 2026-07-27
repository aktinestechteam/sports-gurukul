using SportsGurukul.Platform.Competition.Models;
using SportsGurukul.Platform.Competition.Models.Enums;

namespace SportsGurukul.Platform.Competition.Tests;

public static class TestHelpers
{
    public static IReadOnlyList<Participant> CreateParticipants(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => new Participant
            {
                Id = Guid.NewGuid(),
                Name = $"Participant {i}",
                Ranking = i,
                Region = i % 3 == 0 ? "North" : i % 3 == 1 ? "South" : "East",
                AcademyId = Guid.NewGuid()
            })
            .ToList();
    }

    public static CompetitionConfig CreateConfig(
        CompetitionFormat format = CompetitionFormat.SingleElimination,
        SeedingStrategy seeding = SeedingStrategy.RankingBased,
        int? groupsCount = null)
    {
        return new CompetitionConfig
        {
            TournamentId = Guid.NewGuid(),
            Format = format,
            SeedingStrategy = seeding,
            GroupsCount = groupsCount,
            PointsForWin = 3,
            PointsForDraw = 1,
            PointsForLoss = 0
        };
    }

    public static CompetitionMatch CreateCompletedMatch(
        Guid? homeId = null, Guid? awayId = null,
        int homeScore = 2, int awayScore = 1)
    {
        var hId = homeId ?? Guid.NewGuid();
        var aId = awayId ?? Guid.NewGuid();
        return new CompetitionMatch
        {
            Id = Guid.NewGuid(),
            HomeParticipantId = hId,
            HomeParticipantName = "Home",
            AwayParticipantId = aId,
            AwayParticipantName = "Away",
            HomeScore = homeScore,
            AwayScore = awayScore,
            Status = MatchStatus.Completed,
            WinnerId = homeScore > awayScore ? hId : aId,
            WinnerName = homeScore > awayScore ? "Home" : "Away"
        };
    }
}
