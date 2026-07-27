using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using TournamentTestShared;

namespace Tournament.Domain.Tests.Entities;

public class TournamentRegistrationTests
{
    [Fact]
    public void TournamentRegistration_ExtendsBaseEntity_ShouldHaveBaseProperties()
    {
        var registration = new TournamentRegistration();

        registration.Should().BeAssignableTo<BaseEntity>();
        registration.Id.Should().Be(Guid.Empty);
        registration.CreatedAt.Should().Be(default(DateTime));
        registration.UpdatedAt.Should().BeNull();
        registration.CreatedBy.Should().BeNull();
        registration.UpdatedBy.Should().BeNull();
        registration.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void TournamentRegistration_DefaultValues_AreCorrect()
    {
        var registration = new TournamentRegistration();

        registration.TournamentId.Should().Be(Guid.Empty);
        registration.CategoryId.Should().BeNull();
        registration.DivisionId.Should().BeNull();
        registration.RegistrationStatus.Should().Be(TournamentRegistrationStatus.Pending);
        registration.AthleteId.Should().BeNull();
        registration.TeamId.Should().BeNull();
        registration.AcademyId.Should().BeNull();
        registration.RegistrantName.Should().BeEmpty();
        registration.Email.Should().BeNull();
        registration.Phone.Should().BeNull();
        registration.FeePaid.Should().BeNull();
        registration.PaymentDate.Should().BeNull();
        registration.CheckedInDate.Should().BeNull();
        registration.Notes.Should().BeNull();
        registration.RowVersion.Should().BeEmpty();
    }

    [Fact]
    public void TournamentRegistration_CanSetProperties()
    {
        var registration = new TournamentRegistration
        {
            TournamentId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            DivisionId = Guid.NewGuid(),
            RegistrationStatus = TournamentRegistrationStatus.Approved,
            AthleteId = Guid.NewGuid(),
            AcademyId = Guid.NewGuid(),
            RegistrantName = "John Doe",
            Email = "john@example.com",
            Phone = "+1234567890",
            FeePaid = 100.00m,
            PaymentDate = DateTime.UtcNow,
            Notes = "Early bird registration"
        };

        registration.TournamentId.Should().NotBe(Guid.Empty);
        registration.CategoryId.Should().NotBeNull();
        registration.DivisionId.Should().NotBeNull();
        registration.RegistrationStatus.Should().Be(TournamentRegistrationStatus.Approved);
        registration.AthleteId.Should().NotBeNull();
        registration.AcademyId.Should().NotBeNull();
        registration.RegistrantName.Should().Be("John Doe");
        registration.Email.Should().Be("john@example.com");
        registration.Phone.Should().Be("+1234567890");
        registration.FeePaid.Should().Be(100.00m);
        registration.PaymentDate.Should().NotBeNull();
        registration.Notes.Should().Be("Early bird registration");
    }

    [Fact]
    public void TournamentRegistration_DefaultStatus_IsPending()
    {
        var registration = new TournamentRegistration();

        registration.RegistrationStatus.Should().Be(TournamentRegistrationStatus.Pending);
    }

    [Fact]
    public void TournamentRegistration_CanApprove()
    {
        var registration = TestDataBuilder.CreateRegistration();

        registration.RegistrationStatus = TournamentRegistrationStatus.Approved;

        registration.RegistrationStatus.Should().Be(TournamentRegistrationStatus.Approved);
    }

    [Fact]
    public void TournamentRegistration_CanReject()
    {
        var registration = TestDataBuilder.CreateRegistration();

        registration.RegistrationStatus = TournamentRegistrationStatus.Rejected;

        registration.RegistrationStatus.Should().Be(TournamentRegistrationStatus.Rejected);
    }

    [Fact]
    public void TournamentRegistration_CanCancel()
    {
        var registration = TestDataBuilder.CreateRegistration();

        registration.RegistrationStatus = TournamentRegistrationStatus.Cancelled;

        registration.RegistrationStatus.Should().Be(TournamentRegistrationStatus.Cancelled);
    }

    [Fact]
    public void TournamentRegistration_CanCheckIn()
    {
        var registration = TestDataBuilder.CreateRegistration();
        var checkInDate = DateTime.UtcNow;

        registration.CheckedInDate = checkInDate;
        registration.RegistrationStatus = TournamentRegistrationStatus.CheckedIn;

        registration.CheckedInDate.Should().Be(checkInDate);
        registration.RegistrationStatus.Should().Be(TournamentRegistrationStatus.CheckedIn);
    }

    [Fact]
    public void TournamentRegistration_CanAssignTeam()
    {
        var registration = TestDataBuilder.CreateRegistration();
        var teamId = Guid.NewGuid();

        registration.TeamId = teamId;
        registration.AthleteId = null;

        registration.TeamId.Should().Be(teamId);
        registration.AthleteId.Should().BeNull();
    }

    [Fact]
    public void TournamentRegistration_RowVersion_CanBeSet()
    {
        var registration = new TournamentRegistration();
        var rowVersion = new byte[] { 1, 2, 3, 4, 5 };

        registration.RowVersion = rowVersion;

        registration.RowVersion.Should().BeEquivalentTo(rowVersion);
    }
}
