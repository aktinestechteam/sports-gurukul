using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.CoachManagement.Commands.CreateCoach;
using SportsGurukul.Application.Features.CoachManagement.Validators;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Validators;

public class CreateCoachValidatorTests
{
    private readonly CreateCoachValidator _validator = new();

    [Fact]
    public async Task Valid_Command_Passes()
    {
        var command = new CreateCoachCommand
        {
            UserId = Guid.NewGuid(),
            Biography = "Test biography",
            YearsOfExperience = 5,
            CurrentOrganization = "Test Academy",
            HighestQualification = "BPEd",
            PreferredLanguage = "English",
            CoachingLevel = CoachingLevel.Senior
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task UserId_Empty_Fails()
    {
        var command = new CreateCoachCommand
        {
            UserId = Guid.Empty,
            YearsOfExperience = 5
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID is required.");
    }

    [Fact]
    public async Task NegativeExperience_Fails()
    {
        var command = new CreateCoachCommand
        {
            UserId = Guid.NewGuid(),
            YearsOfExperience = -1
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.YearsOfExperience)
            .WithErrorMessage("Years of experience must be non-negative.");
    }
}
