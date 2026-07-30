using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

namespace SportsGurukul.Communication.Application.Tests.Validators;

public class ScheduleNotificationCommandValidatorTests
{
    private readonly ScheduleNotificationCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenIdIsEmpty_ShouldHaveError()
    {
        var command = new ScheduleNotificationCommand(Guid.Empty, DateTime.UtcNow.AddDays(1));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_WhenIdIsValid_ShouldNotHaveErrorForId()
    {
        var command = new ScheduleNotificationCommand(Guid.NewGuid(), DateTime.UtcNow.AddDays(1));

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_WhenScheduledAtIsInFuture_ShouldNotHaveError()
    {
        var command = new ScheduleNotificationCommand(Guid.NewGuid(), DateTime.UtcNow.AddDays(1));

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.ScheduledAt);
    }

    [Fact]
    public void Validate_WhenScheduledAtIsInPast_ShouldHaveError()
    {
        var command = new ScheduleNotificationCommand(Guid.NewGuid(), DateTime.UtcNow.AddHours(-1));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ScheduledAt);
    }

    [Fact]
    public void Validate_WhenBothPropertiesValid_ShouldNotHaveAnyErrors()
    {
        var command = new ScheduleNotificationCommand(Guid.NewGuid(), DateTime.UtcNow.AddDays(1));

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
