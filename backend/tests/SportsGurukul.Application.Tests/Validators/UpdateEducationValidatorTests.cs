using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.CoachManagement.Commands.UpdateEducation;
using SportsGurukul.Application.Features.CoachManagement.Validators;

namespace SportsGurukul.Application.Tests.Validators;

public class UpdateEducationValidatorTests
{
    private readonly UpdateEducationValidator _validator = new();

    [Fact]
    public async Task ValidCommand_ShouldNotHaveValidationErrors()
    {
        var command = new UpdateEducationCommand
        {
            EducationId = Guid.NewGuid(),
            Degree = "BPEd",
            Institution = "National Institute of Sports",
            FieldOfStudy = "Sports Science",
            YearCompleted = 2020
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task EmptyEducationId_ShouldHaveValidationError()
    {
        var command = new UpdateEducationCommand
        {
            EducationId = Guid.Empty,
            Degree = "BPEd"
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.EducationId)
            .WithErrorMessage("Education ID is required.");
    }

    [Fact]
    public async Task YearCompletedBelow1950_ShouldHaveValidationError()
    {
        var command = new UpdateEducationCommand
        {
            EducationId = Guid.NewGuid(),
            Degree = "BPEd",
            YearCompleted = 1949
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.YearCompleted)
            .WithErrorMessage("Year completed must be between 1950 and 2100.");
    }
}
