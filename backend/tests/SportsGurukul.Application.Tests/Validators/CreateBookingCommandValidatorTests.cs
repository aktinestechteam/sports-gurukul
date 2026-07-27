using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CreateBooking;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

namespace SportsGurukul.Application.Tests.Validators;

public class CreateBookingCommandValidatorTests
{
    private readonly CreateBookingCommandValidator _validator = new();

    [Fact]
    public void EmptyBookingType_ShouldHaveError()
    {
        var command = new CreateBookingCommand { BookingType = string.Empty };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.BookingType);
    }

    [Fact]
    public void EmptyTitle_ShouldHaveError()
    {
        var command = new CreateBookingCommand
        {
            BookingType = "Training",
            Title = string.Empty
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void EmptyAcademyId_ShouldHaveError()
    {
        var command = new CreateBookingCommand
        {
            BookingType = "Training",
            Title = "Test",
            AcademyId = Guid.Empty
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.AcademyId);
    }

    [Fact]
    public void StartTimeAfterEndTime_ShouldHaveError()
    {
        var command = new CreateBookingCommand
        {
            BookingType = "Training",
            Title = "Test",
            AcademyId = Guid.NewGuid(),
            StartTime = TimeSpan.FromHours(10),
            EndTime = TimeSpan.FromHours(9)
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void ValidCommand_ShouldNotHaveErrors()
    {
        var command = new CreateBookingCommand
        {
            BookingType = "Training",
            Title = "Morning Session",
            AcademyId = Guid.NewGuid(),
            BookingDate = DateTime.UtcNow.AddDays(1),
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(10)
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
