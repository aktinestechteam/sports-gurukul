using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.CoachManagement.Commands.UpdateCoachProfile;
using SportsGurukul.Application.Features.CoachManagement.Validators;

namespace SportsGurukul.Application.Tests.Validators;

public class CoachUpdateProfileValidatorTests
{
    private readonly UpdateCoachProfileValidator _validator = new();

    [Fact]
    public async Task ValidCommand_ShouldNotHaveValidationErrors()
    {
        var command = new UpdateCoachProfileCommand
        {
            CoachId = Guid.NewGuid(),
            YearsOfExperience = 5,
            Biography = "Test biography",
            CurrentOrganization = "Test Academy",
            HighestQualification = "BPEd",
            PreferredLanguage = "English"
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task EmptyCoachId_ShouldHaveValidationError()
    {
        var command = new UpdateCoachProfileCommand
        {
            CoachId = Guid.Empty,
            YearsOfExperience = 5
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CoachId)
            .WithErrorMessage("Coach ID is required.");
    }

    [Fact]
    public async Task NegativeYearsOfExperience_ShouldHaveValidationError()
    {
        var command = new UpdateCoachProfileCommand
        {
            CoachId = Guid.NewGuid(),
            YearsOfExperience = -1
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.YearsOfExperience)
            .WithErrorMessage("Years of experience must be non-negative.");
    }

    [Fact]
    public async Task BiographyExceedsMaxLength_ShouldHaveValidationError()
    {
        var command = new UpdateCoachProfileCommand
        {
            CoachId = Guid.NewGuid(),
            Biography = new string('x', 2001)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Biography)
            .WithErrorMessage("Biography must not exceed 2000 characters.");
    }

    [Fact]
    public async Task CurrentOrganizationExceedsMaxLength_ShouldHaveValidationError()
    {
        var command = new UpdateCoachProfileCommand
        {
            CoachId = Guid.NewGuid(),
            CurrentOrganization = new string('x', 201)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CurrentOrganization)
            .WithErrorMessage("Current organization must not exceed 200 characters.");
    }

    [Fact]
    public async Task HighestQualificationExceedsMaxLength_ShouldHaveValidationError()
    {
        var command = new UpdateCoachProfileCommand
        {
            CoachId = Guid.NewGuid(),
            HighestQualification = new string('x', 201)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.HighestQualification)
            .WithErrorMessage("Highest qualification must not exceed 200 characters.");
    }

    [Fact]
    public async Task PreferredLanguageExceedsMaxLength_ShouldHaveValidationError()
    {
        var command = new UpdateCoachProfileCommand
        {
            CoachId = Guid.NewGuid(),
            PreferredLanguage = new string('x', 51)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.PreferredLanguage)
            .WithErrorMessage("Preferred language must not exceed 50 characters.");
    }

    [Fact]
    public async Task NullYearsOfExperience_ShouldNotHaveValidationError()
    {
        var command = new UpdateCoachProfileCommand
        {
            CoachId = Guid.NewGuid(),
            YearsOfExperience = null
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(x => x.YearsOfExperience);
    }
}
