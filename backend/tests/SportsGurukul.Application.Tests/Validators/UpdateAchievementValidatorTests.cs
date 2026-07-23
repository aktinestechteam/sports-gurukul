using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.AthleteManagement.Validators;
using SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateAchievement;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Validators;

public class UpdateAchievementValidatorTests
{
    private readonly UpdateAchievementValidator _validator = new();

    [Fact]
    public async Task ValidCommand_ShouldNotHaveValidationErrors()
    {
        var command = new UpdateAchievementCommand
        {
            AthleteId = Guid.NewGuid(),
            AchievementId = Guid.NewGuid(),
            Title = "Updated Title",
            Level = AchievementLevel.National
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task EmptyAthleteId_ShouldHaveValidationError()
    {
        var command = new UpdateAchievementCommand
        {
            AthleteId = Guid.Empty,
            AchievementId = Guid.NewGuid()
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.AthleteId)
            .WithErrorMessage("Athlete ID is required.");
    }

    [Fact]
    public async Task EmptyAchievementId_ShouldHaveValidationError()
    {
        var command = new UpdateAchievementCommand
        {
            AthleteId = Guid.NewGuid(),
            AchievementId = Guid.Empty
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.AchievementId)
            .WithErrorMessage("Achievement ID is required.");
    }
}
