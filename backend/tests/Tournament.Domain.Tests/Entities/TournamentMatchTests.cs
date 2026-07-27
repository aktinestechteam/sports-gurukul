using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using TournamentTestShared;

namespace Tournament.Domain.Tests.Entities;

public class TournamentMatchTests
{
    [Fact]
    public void TournamentMatch_ExtendsBaseEntity_ShouldHaveBaseProperties()
    {
        var match = new TournamentMatch();

        match.Should().BeAssignableTo<BaseEntity>();
        match.Id.Should().Be(Guid.Empty);
        match.CreatedAt.Should().Be(default(DateTime));
        match.UpdatedAt.Should().BeNull();
        match.CreatedBy.Should().BeNull();
        match.UpdatedBy.Should().BeNull();
        match.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void TournamentMatch_DefaultValues_AreCorrect()
    {
        var match = new TournamentMatch();

        match.TournamentId.Should().Be(Guid.Empty);
        match.TournamentStageId.Should().BeNull();
        match.TournamentRoundId.Should().BeNull();
        match.TournamentVenueId.Should().BeNull();
        match.TournamentCourtId.Should().BeNull();
        match.MatchNumber.Should().Be(0);
        match.HomeParticipantId.Should().BeNull();
        match.AwayParticipantId.Should().BeNull();
        match.HomeParticipantName.Should().BeNull();
        match.AwayParticipantName.Should().BeNull();
        match.ScheduledDate.Should().BeNull();
        match.ScheduledTime.Should().BeNull();
        match.Status.Should().Be(MatchStatus.Scheduled);
        match.HomeScore.Should().BeNull();
        match.AwayScore.Should().BeNull();
        match.ScoreDetails.Should().BeNull();
        match.WinnerId.Should().BeNull();
        match.WinnerName.Should().BeNull();
        match.Notes.Should().BeNull();
        match.RowVersion.Should().BeEmpty();
    }

    [Fact]
    public void TournamentMatch_DefaultCollections_AreNotNull()
    {
        var match = new TournamentMatch();

        match.Sets.Should().NotBeNull().And.BeEmpty();
        match.Results.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void TournamentMatch_CanSetProperties()
    {
        var match = new TournamentMatch
        {
            TournamentId = Guid.NewGuid(),
            TournamentStageId = Guid.NewGuid(),
            TournamentRoundId = Guid.NewGuid(),
            TournamentVenueId = Guid.NewGuid(),
            TournamentCourtId = Guid.NewGuid(),
            MatchNumber = 5,
            HomeParticipantId = Guid.NewGuid(),
            AwayParticipantId = Guid.NewGuid(),
            HomeParticipantName = "Team A",
            AwayParticipantName = "Team B",
            ScheduledDate = new DateTime(2025, 7, 1),
            ScheduledTime = new TimeSpan(14, 30, 0),
            Status = MatchStatus.InProgress,
            HomeScore = 3,
            AwayScore = 2,
            ScoreDetails = "25-21, 21-19, 19-25",
            Notes = "Quarter-final match"
        };

        match.TournamentId.Should().NotBe(Guid.Empty);
        match.TournamentStageId.Should().NotBeNull();
        match.TournamentRoundId.Should().NotBeNull();
        match.TournamentVenueId.Should().NotBeNull();
        match.TournamentCourtId.Should().NotBeNull();
        match.MatchNumber.Should().Be(5);
        match.HomeParticipantName.Should().Be("Team A");
        match.AwayParticipantName.Should().Be("Team B");
        match.ScheduledDate.Should().Be(new DateTime(2025, 7, 1));
        match.ScheduledTime.Should().Be(new TimeSpan(14, 30, 0));
        match.Status.Should().Be(MatchStatus.InProgress);
        match.HomeScore.Should().Be(3);
        match.AwayScore.Should().Be(2);
        match.ScoreDetails.Should().Be("25-21, 21-19, 19-25");
        match.Notes.Should().Be("Quarter-final match");
    }

    [Fact]
    public void TournamentMatch_CanSetWinner()
    {
        var homeParticipantId = Guid.NewGuid();
        var match = TestDataBuilder.CreateMatch(MatchStatus.InProgress);
        match.HomeParticipantId = homeParticipantId;

        match.WinnerId = homeParticipantId;
        match.WinnerName = "Team A";

        match.WinnerId.Should().Be(homeParticipantId);
        match.WinnerName.Should().Be("Team A");
    }

    [Fact]
    public void TournamentMatch_CanUpdateScore()
    {
        var match = TestDataBuilder.CreateMatch(MatchStatus.InProgress);

        match.HomeScore = 3;
        match.AwayScore = 2;

        match.HomeScore.Should().Be(3);
        match.AwayScore.Should().Be(2);
    }

    [Fact]
    public void TournamentMatch_CanTransitionStatus()
    {
        var match = TestDataBuilder.CreateMatch(MatchStatus.Scheduled);

        match.Status.Should().Be(MatchStatus.Scheduled);

        match.Status = MatchStatus.InProgress;
        match.Status.Should().Be(MatchStatus.InProgress);

        match.Status = MatchStatus.Completed;
        match.Status.Should().Be(MatchStatus.Completed);
    }

    [Fact]
    public void TournamentMatch_CanSetNavigationProperties()
    {
        var match = new TournamentMatch
        {
            HomeParticipant = new TournamentParticipant { ParticipantName = "Home" },
            AwayParticipant = new TournamentParticipant { ParticipantName = "Away" },
            Winner = new TournamentParticipant { ParticipantName = "Winner" }
        };

        match.HomeParticipant.Should().NotBeNull();
        match.HomeParticipant!.ParticipantName.Should().Be("Home");
        match.AwayParticipant.Should().NotBeNull();
        match.AwayParticipant!.ParticipantName.Should().Be("Away");
        match.Winner.Should().NotBeNull();
        match.Winner!.ParticipantName.Should().Be("Winner");
    }

    [Fact]
    public void TournamentMatch_RowVersion_CanBeSet()
    {
        var match = new TournamentMatch();
        var rowVersion = new byte[] { 1, 2, 3, 4, 5 };

        match.RowVersion = rowVersion;

        match.RowVersion.Should().BeEquivalentTo(rowVersion);
    }
}
