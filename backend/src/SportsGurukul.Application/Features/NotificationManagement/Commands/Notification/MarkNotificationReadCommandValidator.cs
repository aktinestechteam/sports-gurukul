using FluentValidation;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

public class MarkNotificationReadCommandValidator : AbstractValidator<MarkNotificationReadCommand>
{
    public MarkNotificationReadCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
