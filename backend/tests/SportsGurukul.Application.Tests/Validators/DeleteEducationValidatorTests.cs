using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.CoachManagement.Commands.DeleteEducation;
using SportsGurukul.Application.Features.CoachManagement.Validators;

namespace SportsGurukul.Application.Tests.Validators;

public class DeleteEducationValidatorTests
{
    private readonly DeleteEducationValidator _validator = new();

    [Fact]
    public async Task ValidCommand_ShouldNotHaveValidationErrors()
    {
        var command = new DeleteEducationCommand { EducationId = Guid.NewGuid() };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task EmptyEducationId_ShouldHaveValidationError()
    {
        var command = new DeleteEducationCommand { EducationId = Guid.Empty };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.EducationId)
            .WithErrorMessage("Education ID is required.");
    }
}
