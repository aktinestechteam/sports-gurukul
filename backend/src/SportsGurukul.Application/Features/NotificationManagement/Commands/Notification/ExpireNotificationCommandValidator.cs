using FluentValidation;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

public class ExpireNotificationCommandValidator : AbstractValidator<ExpireNotificationCommand>
{
    public ExpireNotificationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
