using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.ResolveBookingConflict;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

namespace SportsGurukul.Application.Tests.Validators;

public class ResolveBookingConflictCommandValidatorTests
{
    private readonly ResolveBookingConflictCommandValidator _validator = new();

    [Fact]
    public void EmptyConflictId_ShouldHaveError()
    {
        var command = new ResolveBookingConflictCommand { ConflictId = Guid.Empty };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ConflictId);
    }

    [Fact]
    public void EmptyResolutionNotes_ShouldHaveError()
    {
        var command = new ResolveBookingConflictCommand
        {
            ConflictId = Guid.NewGuid(),
            ResolutionNotes = string.Empty
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ResolutionNotes);
    }

    [Fact]
    public void ValidCommand_ShouldNotHaveErrors()
    {
        var command = new ResolveBookingConflictCommand
        {
            ConflictId = Guid.NewGuid(),
            ResolutionNotes = "Resolved by rescheduling"
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
