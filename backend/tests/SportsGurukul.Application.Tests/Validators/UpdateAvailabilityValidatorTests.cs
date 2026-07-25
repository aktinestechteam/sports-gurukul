using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.CoachManagement.Commands.UpdateAvailability;
using SportsGurukul.Application.Features.CoachManagement.Validators;

namespace SportsGurukul.Application.Tests.Validators;

public class UpdateAvailabilityValidatorTests
{
    private readonly UpdateAvailabilityValidator _validator = new();

    [Fact]
    public async Task ValidCommand_ShouldNotHaveValidationErrors()
    {
        var command = new UpdateAvailabilityCommand
        {
            CoachId = Guid.NewGuid(),
            WeeklySchedule = "Mon-Fri 9AM-5PM",
            TimeSlots = "Morning, Afternoon",
            TravelDistance = 50
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task EmptyCoachId_ShouldHaveValidationError()
    {
        var command = new UpdateAvailabilityCommand
        {
            CoachId = Guid.Empty,
            WeeklySchedule = "Mon-Fri 9AM-5PM"
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CoachId)
            .WithErrorMessage("Coach ID is required.");
    }

    [Fact]
    public async Task WeeklyScheduleExceedsMaxLength_ShouldHaveValidationError()
    {
        var command = new UpdateAvailabilityCommand
        {
            CoachId = Guid.NewGuid(),
            WeeklySchedule = new string('x', 5001)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.WeeklySchedule)
            .WithErrorMessage("Weekly schedule must not exceed 5000 characters.");
    }

    [Fact]
    public async Task TimeSlotsExceedsMaxLength_ShouldHaveValidationError()
    {
        var command = new UpdateAvailabilityCommand
        {
            CoachId = Guid.NewGuid(),
            TimeSlots = new string('x', 5001)
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.TimeSlots)
            .WithErrorMessage("Time slots must not exceed 5000 characters.");
    }

    [Fact]
    public async Task NegativeTravelDistance_ShouldHaveValidationError()
    {
        var command = new UpdateAvailabilityCommand
        {
            CoachId = Guid.NewGuid(),
            TravelDistance = -1
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.TravelDistance)
            .WithErrorMessage("Travel distance must be non-negative.");
    }
}
