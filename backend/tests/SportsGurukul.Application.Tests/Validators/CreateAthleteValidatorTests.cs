using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.AthleteManagement.Validators;
using SportsGurukul.Application.Features.AthleteManagement.Commands.CreateAthlete;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Validators;

public class CreateAthleteValidatorTests
{
    private readonly CreateAthleteValidator _validator = new();

    [Fact]
    public async Task ValidCommand_ShouldNotHaveValidationErrors()
    {
        var command = new CreateAthleteCommand
        {
            UserId = Guid.NewGuid(),
            ExperienceYears = 5,
            Height = "175cm",
            Weight = "70kg",
            Biography = "Test bio"
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task EmptyUserId_ShouldHaveValidationError()
    {
        var command = new CreateAthleteCommand { UserId = Guid.Empty, ExperienceYears = 5 };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID is required.");
    }

    [Fact]
    public async Task NegativeExperience_ShouldHaveValidationError()
    {
        var command = new CreateAthleteCommand { UserId = Guid.NewGuid(), ExperienceYears = -1 };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.ExperienceYears)
            .WithErrorMessage("Experience years must be non-negative.");
    }

    [Fact]
    public async Task ZeroExperience_ShouldNotHaveValidationError()
    {
        var command = new CreateAthleteCommand { UserId = Guid.NewGuid(), ExperienceYears = 0 };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(x => x.ExperienceYears);
    }

    [Fact]
    public async Task HeightExceedsMaxLength_ShouldHaveValidationError()
    {
        var command = new CreateAthleteCommand
        {
            UserId = Guid.NewGuid(),
            ExperienceYears = 5,
            Height = new string('x', 21)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Height)
            .WithErrorMessage("Height must not exceed 20 characters.");
    }

    [Fact]
    public async Task WeightExceedsMaxLength_ShouldHaveValidationError()
    {
        var command = new CreateAthleteCommand
        {
            UserId = Guid.NewGuid(),
            ExperienceYears = 5,
            Weight = new string('x', 21)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Weight)
            .WithErrorMessage("Weight must not exceed 20 characters.");
    }

    [Fact]
    public async Task BiographyExceedsMaxLength_ShouldHaveValidationError()
    {
        var command = new CreateAthleteCommand
        {
            UserId = Guid.NewGuid(),
            ExperienceYears = 5,
            Biography = new string('x', 2001)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Biography)
            .WithErrorMessage("Biography must not exceed 2000 characters.");
    }
}
