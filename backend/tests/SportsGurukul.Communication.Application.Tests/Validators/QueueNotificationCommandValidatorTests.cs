using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

namespace SportsGurukul.Communication.Application.Tests.Validators;

public class QueueNotificationCommandValidatorTests
{
    private readonly QueueNotificationCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenIdIsEmpty_ShouldHaveError()
    {
        var command = new QueueNotificationCommand(Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_WhenIdIsValid_ShouldNotHaveErrors()
    {
        var command = new QueueNotificationCommand(Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
