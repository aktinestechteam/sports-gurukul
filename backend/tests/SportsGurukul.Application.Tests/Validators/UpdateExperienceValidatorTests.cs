using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.CoachManagement.Commands.UpdateExperience;
using SportsGurukul.Application.Features.CoachManagement.Validators;

namespace SportsGurukul.Application.Tests.Validators;

public class UpdateExperienceValidatorTests
{
    private readonly UpdateExperienceValidator _validator = new();

    [Fact]
    public async Task ValidCommand_ShouldNotHaveValidationErrors()
    {
        var command = new UpdateExperienceCommand
        {
            ExperienceId = Guid.NewGuid(),
            Organization = "Test Academy",
            Role = "Head Coach",
            Sport = "Cricket",
            Description = "Led the team to victory."
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task EmptyExperienceId_ShouldHaveValidationError()
    {
        var command = new UpdateExperienceCommand
        {
            ExperienceId = Guid.Empty,
            Organization = "Test Academy"
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.ExperienceId)
            .WithErrorMessage("Experience ID is required.");
    }

    [Fact]
    public async Task OrganizationExceedsMaxLength_ShouldHaveValidationError()
    {
        var command = new UpdateExperienceCommand
        {
            ExperienceId = Guid.NewGuid(),
            Organization = new string('x', 201)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Organization)
            .WithErrorMessage("Organization must not exceed 200 characters.");
    }
}
