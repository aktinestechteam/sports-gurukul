using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CreateRecurringBooking;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

namespace SportsGurukul.Application.Tests.Validators;

public class CreateRecurringBookingCommandValidatorTests
{
    private readonly CreateRecurringBookingCommandValidator _validator = new();

    [Fact]
    public void EmptyBookingType_ShouldHaveError()
    {
        var command = new CreateRecurringBookingCommand { BookingType = string.Empty };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.BookingType);
    }

    [Fact]
    public void EmptyRecurrenceType_ShouldHaveError()
    {
        var command = new CreateRecurringBookingCommand
        {
            BookingType = "Training",
            Title = "Test",
            AcademyId = Guid.NewGuid(),
            RecurrenceType = string.Empty
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.RecurrenceType);
    }

    [Fact]
    public void NeitherOccurrenceCountNorEndDate_ShouldHaveError()
    {
        var command = new CreateRecurringBookingCommand
        {
            BookingType = "Training",
            Title = "Test",
            AcademyId = Guid.NewGuid(),
            RecurrenceType = "Daily",
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(10)
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void ValidCommand_ShouldNotHaveErrors()
    {
        var command = new CreateRecurringBookingCommand
        {
            BookingType = "TrainingSession",
            Title = "Weekly Session",
            AcademyId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow.AddDays(1),
            RecurrenceType = "Weekly",
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(10),
            OccurrenceCount = 10
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
