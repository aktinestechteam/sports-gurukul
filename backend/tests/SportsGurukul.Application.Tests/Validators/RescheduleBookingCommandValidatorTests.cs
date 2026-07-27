using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.RescheduleBooking;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

namespace SportsGurukul.Application.Tests.Validators;

public class RescheduleBookingCommandValidatorTests
{
    private readonly RescheduleBookingCommandValidator _validator = new();

    [Fact]
    public void EmptyBookingId_ShouldHaveError()
    {
        var command = new RescheduleBookingCommand { BookingId = Guid.Empty };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.BookingId);
    }

    [Fact]
    public void NewStartTimeAfterEndTime_ShouldHaveError()
    {
        var command = new RescheduleBookingCommand
        {
            BookingId = Guid.NewGuid(),
            NewDate = DateTime.UtcNow.AddDays(5),
            NewStartTime = TimeSpan.FromHours(10),
            NewEndTime = TimeSpan.FromHours(9)
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void ValidCommand_ShouldNotHaveErrors()
    {
        var command = new RescheduleBookingCommand
        {
            BookingId = Guid.NewGuid(),
            NewDate = DateTime.UtcNow.AddDays(5),
            NewStartTime = TimeSpan.FromHours(9),
            NewEndTime = TimeSpan.FromHours(10)
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
