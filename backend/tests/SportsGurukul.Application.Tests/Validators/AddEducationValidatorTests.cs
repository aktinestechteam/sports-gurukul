using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.CoachManagement.Commands.AddEducation;
using SportsGurukul.Application.Features.CoachManagement.Validators;

namespace SportsGurukul.Application.Tests.Validators;

public class AddEducationValidatorTests
{
    private readonly AddEducationValidator _validator = new();

    [Fact]
    public async Task ValidCommand_ShouldNotHaveValidationErrors()
    {
        var command = new AddEducationCommand
        {
            CoachId = Guid.NewGuid(),
            Degree = "BPEd",
            Institution = "National Institute of Sports",
            FieldOfStudy = "Sports Science",
            YearCompleted = 2020
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task EmptyCoachId_ShouldHaveValidationError()
    {
        var command = new AddEducationCommand
        {
            CoachId = Guid.Empty,
            Degree = "BPEd"
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CoachId)
            .WithErrorMessage("Coach ID is required.");
    }

    [Fact]
    public async Task EmptyDegree_ShouldHaveValidationError()
    {
        var command = new AddEducationCommand
        {
            CoachId = Guid.NewGuid(),
            Degree = string.Empty
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Degree)
            .WithErrorMessage("Degree is required.");
    }

    [Fact]
    public async Task DegreeExceedsMaxLength_ShouldHaveValidationError()
    {
        var command = new AddEducationCommand
        {
            CoachId = Guid.NewGuid(),
            Degree = new string('x', 201)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Degree)
            .WithErrorMessage("Degree must not exceed 200 characters.");
    }

    [Fact]
    public async Task InstitutionExceedsMaxLength_ShouldHaveValidationError()
    {
        var command = new AddEducationCommand
        {
            CoachId = Guid.NewGuid(),
            Degree = "BPEd",
            Institution = new string('x', 301)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Institution)
            .WithErrorMessage("Institution must not exceed 300 characters.");
    }

    [Fact]
    public async Task FieldOfStudyExceedsMaxLength_ShouldHaveValidationError()
    {
        var command = new AddEducationCommand
        {
            CoachId = Guid.NewGuid(),
            Degree = "BPEd",
            FieldOfStudy = new string('x', 201)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.FieldOfStudy)
            .WithErrorMessage("Field of study must not exceed 200 characters.");
    }

    [Fact]
    public async Task YearCompletedBelow1950_ShouldHaveValidationError()
    {
        var command = new AddEducationCommand
        {
            CoachId = Guid.NewGuid(),
            Degree = "BPEd",
            YearCompleted = 1949
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.YearCompleted)
            .WithErrorMessage("Year completed must be between 1950 and 2100.");
    }

    [Fact]
    public async Task YearCompletedAbove2100_ShouldHaveValidationError()
    {
        var command = new AddEducationCommand
        {
            CoachId = Guid.NewGuid(),
            Degree = "BPEd",
            YearCompleted = 2101
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.YearCompleted)
            .WithErrorMessage("Year completed must be between 1950 and 2100.");
    }
}
