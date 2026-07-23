using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.AthleteManagement.Validators;
using SportsGurukul.Application.Features.AthleteManagement.Commands.DeleteAthlete;

namespace SportsGurukul.Application.Tests.Validators;

public class DeleteAthleteValidatorTests
{
    private readonly DeleteAthleteValidator _validator = new();

    [Fact]
    public async Task ValidCommand_ShouldNotHaveValidationErrors()
    {
        var command = new DeleteAthleteCommand { AthleteId = Guid.NewGuid() };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task EmptyAthleteId_ShouldHaveValidationError()
    {
        var command = new DeleteAthleteCommand { AthleteId = Guid.Empty };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.AthleteId)
            .WithErrorMessage("Athlete ID is required.");
    }
}
