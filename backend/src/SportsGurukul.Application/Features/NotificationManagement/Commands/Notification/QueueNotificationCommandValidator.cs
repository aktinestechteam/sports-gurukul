using FluentValidation;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

public class QueueNotificationCommandValidator : AbstractValidator<QueueNotificationCommand>
{
    public QueueNotificationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
