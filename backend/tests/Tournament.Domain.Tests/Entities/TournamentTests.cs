using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace Tournament.Domain.Tests.Entities;

public class TournamentTests
{
    [Fact]
    public void Tournament_ExtendsBaseEntity_ShouldHaveBaseProperties()
    {
        var tournament = new TournamentEntity();

        tournament.Should().BeAssignableTo<BaseEntity>();
        tournament.Id.Should().Be(Guid.Empty);
        tournament.CreatedAt.Should().Be(default(DateTime));
        tournament.UpdatedAt.Should().BeNull();
        tournament.CreatedBy.Should().BeNull();
        tournament.UpdatedBy.Should().BeNull();
        tournament.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Tournament_DefaultValues_AreCorrect()
    {
        var tournament = new TournamentEntity();

        tournament.TournamentCode.Should().BeEmpty();
        tournament.TournamentName.Should().BeEmpty();
        tournament.Description.Should().BeNull();
        tournament.TournamentType.Should().Be(default(TournamentType));
        tournament.Status.Should().Be(TournamentStatus.Draft);
        tournament.IsPublished.Should().BeFalse();
        tournament.RegistrationType.Should().Be(default(RegistrationType));
        tournament.MaxParticipants.Should().BeNull();
        tournament.MinParticipants.Should().BeNull();
        tournament.RegistrationFee.Should().BeNull();
        tournament.Venue.Should().BeNull();
        tournament.Rules.Should().BeNull();
        tournament.ContactEmail.Should().BeNull();
        tournament.ContactPhone.Should().BeNull();
        tournament.Website.Should().BeNull();
        tournament.RowVersion.Should().BeEmpty();
    }

    [Fact]
    public void Tournament_DefaultCollections_AreNotNull()
    {
        var tournament = new TournamentEntity();

        tournament.Categories.Should().NotBeNull().And.BeEmpty();
        tournament.TournamentSports.Should().NotBeNull().And.BeEmpty();
        tournament.Venues.Should().NotBeNull().And.BeEmpty();
        tournament.Stages.Should().NotBeNull().And.BeEmpty();
        tournament.Registrations.Should().NotBeNull().And.BeEmpty();
        tournament.Participants.Should().NotBeNull().And.BeEmpty();
        tournament.Teams.Should().NotBeNull().And.BeEmpty();
        tournament.Officials.Should().NotBeNull().And.BeEmpty();
        tournament.Sponsors.Should().NotBeNull().And.BeEmpty();
        tournament.Documents.Should().NotBeNull().And.BeEmpty();
        tournament.Gallery.Should().NotBeNull().And.BeEmpty();
        tournament.Rules_.Should().NotBeNull().And.BeEmpty();
        tournament.Rankings.Should().NotBeNull().And.BeEmpty();
        tournament.Awards.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Tournament_CanSetProperties()
    {
        var tournament = new TournamentEntity
        {
            TournamentCode = "TRN-001",
            TournamentName = "Summer Championship",
            Description = "Annual summer tournament",
            AcademyId = Guid.NewGuid(),
            SportId = Guid.NewGuid(),
            TournamentType = TournamentType.Knockout,
            Status = TournamentStatus.Published,
            StartDate = new DateTime(2025, 7, 1),
            EndDate = new DateTime(2025, 7, 15),
            RegistrationOpenDate = new DateTime(2025, 6, 1),
            RegistrationCloseDate = new DateTime(2025, 6, 25),
            MaxParticipants = 64,
            MinParticipants = 16,
            RegistrationFee = 150.00m,
            RegistrationType = RegistrationType.Individual,
            Venue = "Sports Complex",
            Rules = "Standard rules",
            ContactEmail = "info@tournament.com",
            ContactPhone = "+1234567890",
            Website = "https://tournament.com",
            IsPublished = true
        };

        tournament.TournamentCode.Should().Be("TRN-001");
        tournament.TournamentName.Should().Be("Summer Championship");
        tournament.Description.Should().Be("Annual summer tournament");
        tournament.TournamentType.Should().Be(TournamentType.Knockout);
        tournament.Status.Should().Be(TournamentStatus.Published);
        tournament.StartDate.Should().Be(new DateTime(2025, 7, 1));
        tournament.EndDate.Should().Be(new DateTime(2025, 7, 15));
        tournament.RegistrationOpenDate.Should().Be(new DateTime(2025, 6, 1));
        tournament.RegistrationCloseDate.Should().Be(new DateTime(2025, 6, 25));
        tournament.MaxParticipants.Should().Be(64);
        tournament.MinParticipants.Should().Be(16);
        tournament.RegistrationFee.Should().Be(150.00m);
        tournament.RegistrationType.Should().Be(RegistrationType.Individual);
        tournament.Venue.Should().Be("Sports Complex");
        tournament.Rules.Should().Be("Standard rules");
        tournament.ContactEmail.Should().Be("info@tournament.com");
        tournament.ContactPhone.Should().Be("+1234567890");
        tournament.Website.Should().Be("https://tournament.com");
        tournament.IsPublished.Should().BeTrue();
    }

    [Fact]
    public void Tournament_CanSetBaseEntityProperties()
    {
        var id = Guid.NewGuid();
        var createdBy = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var tournament = new TournamentEntity
        {
            Id = id,
            CreatedAt = now,
            UpdatedAt = now.AddDays(1),
            CreatedBy = createdBy,
            UpdatedBy = createdBy,
            IsDeleted = false
        };

        tournament.Id.Should().Be(id);
        tournament.CreatedAt.Should().Be(now);
        tournament.UpdatedAt.Should().Be(now.AddDays(1));
        tournament.CreatedBy.Should().Be(createdBy);
        tournament.UpdatedBy.Should().Be(createdBy);
        tournament.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Tournament_CanAddRegistrations()
    {
        var tournament = new TournamentEntity();
        var registration1 = new TournamentRegistration
        {
            Id = Guid.NewGuid(),
            TournamentId = tournament.Id,
            RegistrantName = "Player 1"
        };
        var registration2 = new TournamentRegistration
        {
            Id = Guid.NewGuid(),
            TournamentId = tournament.Id,
            RegistrantName = "Player 2"
        };

        tournament.Registrations.Add(registration1);
        tournament.Registrations.Add(registration2);

        tournament.Registrations.Should().HaveCount(2);
    }

    [Fact]
    public void Tournament_CanAddParticipants()
    {
        var tournament = new TournamentEntity();
        var participant = new TournamentParticipant
        {
            Id = Guid.NewGuid(),
            TournamentId = tournament.Id,
            ParticipantName = "Team A"
        };

        tournament.Participants.Add(participant);

        tournament.Participants.Should().HaveCount(1);
    }

    [Fact]
    public void Tournament_CanAddRankings()
    {
        var tournament = new TournamentEntity();
        var ranking = new TournamentRanking
        {
            Id = Guid.NewGuid(),
            TournamentId = tournament.Id,
            ParticipantId = Guid.NewGuid(),
            Rank = 1
        };

        tournament.Rankings.Add(ranking);

        tournament.Rankings.Should().HaveCount(1);
    }

    [Fact]
    public void Tournament_CanAddStages()
    {
        var tournament = new TournamentEntity();
        var stage = new TournamentStage
        {
            Id = Guid.NewGuid(),
            TournamentId = tournament.Id,
            StageName = "Group Stage",
            StageOrder = 1
        };

        tournament.Stages.Add(stage);

        tournament.Stages.Should().HaveCount(1);
    }

    [Fact]
    public void Tournament_CanAddAwards()
    {
        var tournament = new TournamentEntity();
        var award = new TournamentAward
        {
            Id = Guid.NewGuid(),
            TournamentId = tournament.Id,
            AwardType = TournamentAwardType.Winner,
            AwardName = "Winner"
        };

        tournament.Awards.Add(award);

        tournament.Awards.Should().HaveCount(1);
    }

    [Fact]
    public void Tournament_RowVersion_CanBeSet()
    {
        var tournament = new TournamentEntity();
        var rowVersion = new byte[] { 1, 2, 3, 4, 5 };

        tournament.RowVersion = rowVersion;

        tournament.RowVersion.Should().BeEquivalentTo(rowVersion);
    }

    [Fact]
    public void Tournament_StatusTransitions_CanBeSet()
    {
        var tournament = new TournamentEntity();

        tournament.Status.Should().Be(TournamentStatus.Draft);

        tournament.Status = TournamentStatus.Published;
        tournament.Status.Should().Be(TournamentStatus.Published);

        tournament.Status = TournamentStatus.RegistrationOpen;
        tournament.Status.Should().Be(TournamentStatus.RegistrationOpen);

        tournament.Status = TournamentStatus.Live;
        tournament.Status.Should().Be(TournamentStatus.Live);

        tournament.Status = TournamentStatus.Completed;
        tournament.Status.Should().Be(TournamentStatus.Completed);
    }
}
