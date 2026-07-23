using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.AthleteManagement.Validators;
using SportsGurukul.Application.Features.AthleteManagement.Commands.DeleteAchievement;
using SportsGurukul.Application.Features.AthleteManagement.Commands.RemoveSport;
using SportsGurukul.Application.Features.AthleteManagement.Commands.RestoreAthlete;
using SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateMedicalProfile;
using SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateEmergencyContact;
using SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateRanking;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Validators;

public class RemainingValidatorTests
{
    [Fact]
    public async Task DeleteAchievement_ValidCommand_NoErrors()
    {
        var validator = new DeleteAchievementValidator();
        var command = new DeleteAchievementCommand
        {
            AthleteId = Guid.NewGuid(),
            AchievementId = Guid.NewGuid()
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task DeleteAchievement_EmptyIds_ShouldHaveErrors()
    {
        var validator = new DeleteAchievementValidator();
        var command = new DeleteAchievementCommand
        {
            AthleteId = Guid.Empty,
            AchievementId = Guid.Empty
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.AthleteId);
        result.ShouldHaveValidationErrorFor(x => x.AchievementId);
    }

    [Fact]
    public async Task RemoveSport_ValidCommand_NoErrors()
    {
        var validator = new RemoveSportValidator();
        var command = new RemoveSportCommand
        {
            AthleteId = Guid.NewGuid(),
            SportId = Guid.NewGuid()
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task RemoveSport_EmptyIds_ShouldHaveErrors()
    {
        var validator = new RemoveSportValidator();
        var command = new RemoveSportCommand
        {
            AthleteId = Guid.Empty,
            SportId = Guid.Empty
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.AthleteId);
        result.ShouldHaveValidationErrorFor(x => x.SportId);
    }

    [Fact]
    public async Task RestoreAthlete_ValidCommand_NoErrors()
    {
        var validator = new RestoreAthleteValidator();
        var command = new RestoreAthleteCommand { AthleteId = Guid.NewGuid() };

        var result = await validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task RestoreAthlete_EmptyId_ShouldHaveError()
    {
        var validator = new RestoreAthleteValidator();
        var command = new RestoreAthleteCommand { AthleteId = Guid.Empty };

        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.AthleteId);
    }

    [Fact]
    public async Task UpdateMedicalProfile_ValidCommand_NoErrors()
    {
        var validator = new UpdateMedicalProfileValidator();
        var command = new UpdateMedicalProfileCommand
        {
            AthleteId = Guid.NewGuid(),
            MedicalConditions = "None"
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task UpdateMedicalProfile_EmptyId_ShouldHaveError()
    {
        var validator = new UpdateMedicalProfileValidator();
        var command = new UpdateMedicalProfileCommand { AthleteId = Guid.Empty };

        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.AthleteId);
    }

    [Fact]
    public async Task UpdateEmergencyContact_ValidCommand_NoErrors()
    {
        var validator = new UpdateEmergencyContactValidator();
        var command = new UpdateEmergencyContactCommand
        {
            AthleteId = Guid.NewGuid(),
            Name = "John",
            Relationship = EmergencyRelationship.Parent,
            Phone = "+1234567890"
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task UpdateEmergencyContact_EmptyId_ShouldHaveError()
    {
        var validator = new UpdateEmergencyContactValidator();
        var command = new UpdateEmergencyContactCommand
        {
            AthleteId = Guid.Empty,
            Name = "John",
            Phone = "+1234567890"
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.AthleteId);
    }

    [Fact]
    public async Task UpdateRanking_ValidCommand_NoErrors()
    {
        var validator = new UpdateRankingValidator();
        var command = new UpdateRankingCommand
        {
            AthleteId = Guid.NewGuid(),
            CurrentRank = "10"
        };

        var result = await validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task UpdateRanking_EmptyId_ShouldHaveError()
    {
        var validator = new UpdateRankingValidator();
        var command = new UpdateRankingCommand { AthleteId = Guid.Empty };

        var result = await validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.AthleteId);
    }
}
