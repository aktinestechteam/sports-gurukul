using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.AthleteManagement.Validators;
using SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateAthlete;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Validators;

public class UpdateAthleteValidatorTests
{
    private readonly UpdateAthleteValidator _validator = new();

    [Fact]
    public async Task ValidCommand_ShouldNotHaveValidationErrors()
    {
        var command = new UpdateAthleteCommand
        {
            AthleteId = Guid.NewGuid(),
            CurrentLevel = AthleteLevel.Advanced,
            ExperienceYears = 10,
            Height = "180cm",
            Weight = "75kg"
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task EmptyAthleteId_ShouldHaveValidationError()
    {
        var command = new UpdateAthleteCommand { AthleteId = Guid.Empty };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.AthleteId)
            .WithErrorMessage("Athlete ID is required.");
    }
}
