using FluentValidation;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

public class CancelNotificationCommandValidator : AbstractValidator<CancelNotificationCommand>
{
    public CancelNotificationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
