using FluentValidation;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

public class RetryNotificationCommandValidator : AbstractValidator<RetryNotificationCommand>
{
    public RetryNotificationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x)
            .Must(HaveValidRetryState)
            .WithMessage("Notification is not in a retryable state");
    }

    private bool HaveValidRetryState(RetryNotificationCommand command)
    {
        return true;
    }
}
