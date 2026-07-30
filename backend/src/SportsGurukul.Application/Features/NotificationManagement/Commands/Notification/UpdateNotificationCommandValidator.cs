using FluentValidation;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

public class UpdateNotificationCommandValidator : AbstractValidator<UpdateNotificationCommand>
{
    public UpdateNotificationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Subject)
            .MaximumLength(500)
            .When(x => x.Subject is not null);

        RuleFor(x => x.ScheduledAt)
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.ScheduledAt.HasValue)
            .WithMessage("Schedule time must be in the future");
    }
}
