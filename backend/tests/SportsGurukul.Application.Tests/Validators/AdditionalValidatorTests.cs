using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.AthleteManagement.Validators;
using SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateEmergencyContact;
using SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateMedicalProfile;
using SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateRanking;
using SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateAchievement;
using SportsGurukul.Application.Features.AthleteManagement.Commands.AddAchievement;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Validators;

public class AdditionalValidatorTests
{
    [Fact]
    public async Task UpdateEmergencyContact_EmptyName_ShouldHaveError()
    {
        var validator = new UpdateEmergencyContactValidator();
        var command = new UpdateEmergencyContactCommand
        {
            AthleteId = Guid.NewGuid(),
            Name = string.Empty,
            Relationship = EmergencyRelationship.Parent,
            Phone = "+1234567890"
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task UpdateEmergencyContact_EmptyPhone_ShouldHaveError()
    {
        var validator = new UpdateEmergencyContactValidator();
        var command = new UpdateEmergencyContactCommand
        {
            AthleteId = Guid.NewGuid(),
            Name = "John",
            Relationship = EmergencyRelationship.Parent,
            Phone = string.Empty
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Phone);
    }

    [Fact]
    public async Task UpdateEmergencyContact_NameExceedsMaxLength_ShouldHaveError()
    {
        var validator = new UpdateEmergencyContactValidator();
        var command = new UpdateEmergencyContactCommand
        {
            AthleteId = Guid.NewGuid(),
            Name = new string('x', 201),
            Relationship = EmergencyRelationship.Parent,
            Phone = "+1234567890"
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task UpdateEmergencyContact_PhoneExceedsMaxLength_ShouldHaveError()
    {
        var validator = new UpdateEmergencyContactValidator();
        var command = new UpdateEmergencyContactCommand
        {
            AthleteId = Guid.NewGuid(),
            Name = "John",
            Relationship = EmergencyRelationship.Parent,
            Phone = new string('9', 51)
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Phone);
    }

    [Fact]
    public async Task UpdateEmergencyContact_InvalidEmail_ShouldHaveError()
    {
        var validator = new UpdateEmergencyContactValidator();
        var command = new UpdateEmergencyContactCommand
        {
            AthleteId = Guid.NewGuid(),
            Name = "John",
            Relationship = EmergencyRelationship.Parent,
            Phone = "+1234567890",
            Email = "not-an-email"
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task UpdateEmergencyContact_ValidEmail_ShouldNotHaveError()
    {
        var validator = new UpdateEmergencyContactValidator();
        var command = new UpdateEmergencyContactCommand
        {
            AthleteId = Guid.NewGuid(),
            Name = "John",
            Relationship = EmergencyRelationship.Parent,
            Phone = "+1234567890",
            Email = "john@example.com"
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task UpdateMedicalProfile_MedicalConditionsExceedsMaxLength_ShouldHaveError()
    {
        var validator = new UpdateMedicalProfileValidator();
        var command = new UpdateMedicalProfileCommand
        {
            AthleteId = Guid.NewGuid(),
            MedicalConditions = new string('x', 2001)
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.MedicalConditions);
    }

    [Fact]
    public async Task UpdateMedicalProfile_InsuranceNumberExceedsMaxLength_ShouldHaveError()
    {
        var validator = new UpdateMedicalProfileValidator();
        var command = new UpdateMedicalProfileCommand
        {
            AthleteId = Guid.NewGuid(),
            InsuranceNumber = new string('x', 101)
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.InsuranceNumber);
    }

    [Fact]
    public async Task UpdateMedicalProfile_DoctorNameExceedsMaxLength_ShouldHaveError()
    {
        var validator = new UpdateMedicalProfileValidator();
        var command = new UpdateMedicalProfileCommand
        {
            AthleteId = Guid.NewGuid(),
            DoctorName = new string('x', 201)
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.DoctorName);
    }

    [Fact]
    public async Task UpdateRanking_CurrentRankExceedsMaxLength_ShouldHaveError()
    {
        var validator = new UpdateRankingValidator();
        var command = new UpdateRankingCommand
        {
            AthleteId = Guid.NewGuid(),
            CurrentRank = new string('1', 51)
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.CurrentRank);
    }

    [Fact]
    public async Task UpdateRanking_RankingAuthorityExceedsMaxLength_ShouldHaveError()
    {
        var validator = new UpdateRankingValidator();
        var command = new UpdateRankingCommand
        {
            AthleteId = Guid.NewGuid(),
            RankingAuthority = new string('x', 201)
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.RankingAuthority);
    }

    [Fact]
    public async Task UpdateAchievement_CompetitionExceedsMaxLength_ShouldHaveError()
    {
        var validator = new UpdateAchievementValidator();
        var command = new UpdateAchievementCommand
        {
            AthleteId = Guid.NewGuid(),
            AchievementId = Guid.NewGuid(),
            Competition = new string('x', 201)
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Competition);
    }

    [Fact]
    public async Task UpdateAchievement_PositionExceedsMaxLength_ShouldHaveError()
    {
        var validator = new UpdateAchievementValidator();
        var command = new UpdateAchievementCommand
        {
            AthleteId = Guid.NewGuid(),
            AchievementId = Guid.NewGuid(),
            Position = new string('x', 101)
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Position);
    }

    [Fact]
    public async Task UpdateAchievement_TitleExceedsMaxLength_ShouldHaveError()
    {
        var validator = new UpdateAchievementValidator();
        var command = new UpdateAchievementCommand
        {
            AthleteId = Guid.NewGuid(),
            AchievementId = Guid.NewGuid(),
            Title = new string('x', 201)
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public async Task AddAchievement_CompetitionExceedsMaxLength_ShouldHaveError()
    {
        var validator = new AddAchievementValidator();
        var command = new AddAchievementCommand
        {
            AthleteId = Guid.NewGuid(),
            Title = "Valid Title",
            Competition = new string('x', 201)
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Competition);
    }

    [Fact]
    public async Task AddAchievement_PositionExceedsMaxLength_ShouldHaveError()
    {
        var validator = new AddAchievementValidator();
        var command = new AddAchievementCommand
        {
            AthleteId = Guid.NewGuid(),
            Title = "Valid Title",
            Position = new string('x', 101)
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Position);
    }
}
