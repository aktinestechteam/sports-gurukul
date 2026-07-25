using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.CoachManagement.Commands.DeleteExperience;
using SportsGurukul.Application.Features.CoachManagement.Validators;

namespace SportsGurukul.Application.Tests.Validators;

public class DeleteExperienceValidatorTests
{
    private readonly DeleteExperienceValidator _validator = new();

    [Fact]
    public async Task ValidCommand_ShouldNotHaveValidationErrors()
    {
        var command = new DeleteExperienceCommand { ExperienceId = Guid.NewGuid() };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task EmptyExperienceId_ShouldHaveValidationError()
    {
        var command = new DeleteExperienceCommand { ExperienceId = Guid.Empty };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.ExperienceId)
            .WithErrorMessage("Experience ID is required.");
    }
}
