using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.AthleteManagement.Validators;
using SportsGurukul.Application.Features.AthleteManagement.Commands.AssignSport;

namespace SportsGurukul.Application.Tests.Validators;

public class AssignSportValidatorTests
{
    private readonly AssignSportValidator _validator = new();

    [Fact]
    public async Task ValidCommand_ShouldNotHaveValidationErrors()
    {
        var command = new AssignSportCommand
        {
            AthleteId = Guid.NewGuid(),
            SportId = Guid.NewGuid()
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task EmptyAthleteId_ShouldHaveValidationError()
    {
        var command = new AssignSportCommand
        {
            AthleteId = Guid.Empty,
            SportId = Guid.NewGuid()
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.AthleteId)
            .WithErrorMessage("Athlete ID is required.");
    }

    [Fact]
    public async Task EmptySportId_ShouldHaveValidationError()
    {
        var command = new AssignSportCommand
        {
            AthleteId = Guid.NewGuid(),
            SportId = Guid.Empty
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.SportId)
            .WithErrorMessage("Sport ID is required.");
    }
}
