using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.CoachManagement.Commands.RemoveSport;
using SportsGurukul.Application.Features.CoachManagement.Validators;

namespace SportsGurukul.Application.Tests.Validators;

public class CoachRemoveSportValidatorTests
{
    private readonly RemoveSportValidator _validator = new();

    [Fact]
    public async Task ValidCommand_ShouldNotHaveValidationErrors()
    {
        var command = new RemoveSportCommand
        {
            CoachId = Guid.NewGuid(),
            SportId = Guid.NewGuid()
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task EmptyCoachId_ShouldHaveValidationError()
    {
        var command = new RemoveSportCommand
        {
            CoachId = Guid.Empty,
            SportId = Guid.NewGuid()
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CoachId)
            .WithErrorMessage("Coach ID is required.");
    }

    [Fact]
    public async Task EmptySportId_ShouldHaveValidationError()
    {
        var command = new RemoveSportCommand
        {
            CoachId = Guid.NewGuid(),
            SportId = Guid.Empty
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.SportId)
            .WithErrorMessage("Sport ID is required.");
    }
}
