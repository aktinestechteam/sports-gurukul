using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.ScheduleReminder;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

namespace SportsGurukul.Application.Tests.Validators;

public class ScheduleReminderCommandValidatorTests
{
    private readonly ScheduleReminderCommandValidator _validator = new();

    [Fact]
    public void EmptyBookingId_ShouldHaveError()
    {
        var command = new ScheduleReminderCommand { BookingId = Guid.Empty };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.BookingId);
    }

    [Fact]
    public void ZeroReminderMinutes_ShouldHaveError()
    {
        var command = new ScheduleReminderCommand
        {
            BookingId = Guid.NewGuid(),
            ReminderMinutesBefore = 0
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ReminderMinutesBefore);
    }

    [Fact]
    public void ValidCommand_ShouldNotHaveErrors()
    {
        var command = new ScheduleReminderCommand
        {
            BookingId = Guid.NewGuid(),
            ReminderMinutesBefore = 30
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
