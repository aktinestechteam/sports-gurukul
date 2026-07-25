using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.CoachManagement.Commands.AssignAthlete;
using SportsGurukul.Application.Features.CoachManagement.Validators;

namespace SportsGurukul.Application.Tests.Validators;

public class AssignAthleteValidatorTests
{
    private readonly AssignAthleteValidator _validator = new();

    [Fact]
    public async Task ValidCommand_ShouldNotHaveValidationErrors()
    {
        var command = new AssignAthleteCommand
        {
            CoachId = Guid.NewGuid(),
            AthleteId = Guid.NewGuid()
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task EmptyCoachId_ShouldHaveValidationError()
    {
        var command = new AssignAthleteCommand
        {
            CoachId = Guid.Empty,
            AthleteId = Guid.NewGuid()
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CoachId)
            .WithErrorMessage("Coach ID is required.");
    }

    [Fact]
    public async Task EmptyAthleteId_ShouldHaveValidationError()
    {
        var command = new AssignAthleteCommand
        {
            CoachId = Guid.NewGuid(),
            AthleteId = Guid.Empty
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.AthleteId)
            .WithErrorMessage("Athlete ID is required.");
    }
}
