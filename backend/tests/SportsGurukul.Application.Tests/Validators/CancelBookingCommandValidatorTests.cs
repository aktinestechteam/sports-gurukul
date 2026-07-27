using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CancelBooking;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

namespace SportsGurukul.Application.Tests.Validators;

public class CancelBookingCommandValidatorTests
{
    private readonly CancelBookingCommandValidator _validator = new();

    [Fact]
    public void EmptyBookingId_ShouldHaveError()
    {
        var command = new CancelBookingCommand { BookingId = Guid.Empty, Reason = "Test" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.BookingId);
    }

    [Fact]
    public void EmptyReason_ShouldHaveError()
    {
        var command = new CancelBookingCommand
        {
            BookingId = Guid.NewGuid(),
            Reason = string.Empty
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Reason);
    }

    [Fact]
    public void ValidCommand_ShouldNotHaveErrors()
    {
        var command = new CancelBookingCommand
        {
            BookingId = Guid.NewGuid(),
            Reason = "Schedule conflict"
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
