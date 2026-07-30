using FluentValidation;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

public class ArchiveNotificationCommandValidator : AbstractValidator<ArchiveNotificationCommand>
{
    public ArchiveNotificationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
