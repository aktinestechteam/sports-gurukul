using FluentAssertions;
using SportsGurukul.Application.Features.AthleteManagement.Commands.CreateAthlete;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Mappings;

public class MapToDtoTests
{
    [Fact]
    public void MapToDto_MapsAllAthleteFields()
    {
        var athlete = TestDataBuilder.CreateAthleteWithDetails();
        var user = athlete.User;

        var dto = CreateAthleteCommandHandler.MapToDto(athlete, user);

        dto.Id.Should().Be(athlete.Id);
        dto.UserId.Should().Be(athlete.UserId);
        dto.AthleteCode.Should().Be(athlete.AthleteCode);
        dto.FullName.Should().Be(user.FullName);
        dto.Email.Should().Be(user.Email);
        dto.PhoneNumber.Should().Be(user.PhoneNumber);
        dto.CurrentLevel.Should().Be(athlete.CurrentLevel.ToString());
        dto.ExperienceYears.Should().Be(athlete.ExperienceYears);
        dto.Height.Should().Be(athlete.Height);
        dto.Weight.Should().Be(athlete.Weight);
        dto.BloodGroup.Should().Be(athlete.BloodGroup.ToString());
        dto.DominantHand.Should().Be(athlete.DominantHand.ToString());
        dto.DominantFoot.Should().Be(athlete.DominantFoot.ToString());
        dto.Biography.Should().Be(athlete.Biography);
        dto.Status.Should().Be(athlete.Status.ToString());
    }

    [Fact]
    public void MapToDto_MapsRoles()
    {
        var athlete = TestDataBuilder.CreateAthleteWithDetails();
        var user = athlete.User;

        var dto = CreateAthleteCommandHandler.MapToDto(athlete, user);

        dto.Roles.Should().ContainSingle(r => r == "Athlete");
    }

    [Fact]
    public void MapToDto_MapsSports()
    {
        var athlete = TestDataBuilder.CreateAthleteWithDetails();
        var user = athlete.User;

        var dto = CreateAthleteCommandHandler.MapToDto(athlete, user);

        dto.Sports.Should().HaveCount(1);
        dto.Sports.First().Name.Should().Be("Cricket");
        dto.Sports.First().IsPrimarySport.Should().BeTrue();
    }

    [Fact]
    public void MapToDto_MapsAchievements()
    {
        var athlete = TestDataBuilder.CreateAthleteWithDetails();
        var user = athlete.User;

        var dto = CreateAthleteCommandHandler.MapToDto(athlete, user);

        dto.Achievements.Should().HaveCount(1);
        dto.Achievements.First().Title.Should().Be("State Championship");
        dto.Achievements.First().Level.Should().Be("State");
    }

    [Fact]
    public void MapToDto_MapsMedicalProfile()
    {
        var athlete = TestDataBuilder.CreateAthleteWithDetails();
        var user = athlete.User;

        var dto = CreateAthleteCommandHandler.MapToDto(athlete, user);

        dto.MedicalProfile.Should().NotBeNull();
        dto.MedicalProfile!.MedicalConditions.Should().Be("None");
        dto.MedicalProfile.InsuranceNumber.Should().Be("INS-001");
        dto.MedicalProfile.DoctorName.Should().Be("Dr. Smith");
    }

    [Fact]
    public void MapToDto_MapsEmergencyContact()
    {
        var athlete = TestDataBuilder.CreateAthleteWithDetails();
        var user = athlete.User;

        var dto = CreateAthleteCommandHandler.MapToDto(athlete, user);

        dto.EmergencyContact.Should().NotBeNull();
        dto.EmergencyContact!.Name.Should().Be("John Doe");
        dto.EmergencyContact.Relationship.Should().Be("Parent");
        dto.EmergencyContact.Phone.Should().Be("+1987654321");
    }

    [Fact]
    public void MapToDto_MapsRanking()
    {
        var athlete = TestDataBuilder.CreateAthleteWithDetails();
        var user = athlete.User;

        var dto = CreateAthleteCommandHandler.MapToDto(athlete, user);

        dto.Ranking.Should().NotBeNull();
        dto.Ranking!.CurrentRank.Should().Be("10");
        dto.Ranking.StateRank.Should().Be("5");
        dto.Ranking.NationalRank.Should().Be("50");
        dto.Ranking.InternationalRank.Should().Be("500");
        dto.Ranking.RankingAuthority.Should().Be("World Athletics");
    }

    [Fact]
    public void MapToDto_NullMedicalProfile_ReturnsNullDto()
    {
        var athlete = TestDataBuilder.CreateAthleteWithDetails();
        athlete.MedicalProfile = null;
        var user = athlete.User;

        var dto = CreateAthleteCommandHandler.MapToDto(athlete, user);

        dto.MedicalProfile.Should().BeNull();
    }

    [Fact]
    public void MapToDto_NullEmergencyContact_ReturnsNullDto()
    {
        var athlete = TestDataBuilder.CreateAthleteWithDetails();
        athlete.EmergencyContact = null;
        var user = athlete.User;

        var dto = CreateAthleteCommandHandler.MapToDto(athlete, user);

        dto.EmergencyContact.Should().BeNull();
    }

    [Fact]
    public void MapToDto_NullRanking_ReturnsNullDto()
    {
        var athlete = TestDataBuilder.CreateAthleteWithDetails();
        athlete.Ranking = null;
        var user = athlete.User;

        var dto = CreateAthleteCommandHandler.MapToDto(athlete, user);

        dto.Ranking.Should().BeNull();
    }

    [Fact]
    public void MapToDto_EmptyCollections_ReturnsEmptyLists()
    {
        var athlete = TestDataBuilder.CreateAthleteWithDetails();
        athlete.AthleteSports = new List<Domain.Entities.AthleteSport>();
        athlete.AthleteAchievements = new List<Domain.Entities.AthleteAchievement>();
        athlete.User.UserRoles = new List<Domain.Entities.UserRole>();
        var user = athlete.User;

        var dto = CreateAthleteCommandHandler.MapToDto(athlete, user);

        dto.Sports.Should().BeEmpty();
        dto.Achievements.Should().BeEmpty();
        dto.Roles.Should().BeEmpty();
    }
}
