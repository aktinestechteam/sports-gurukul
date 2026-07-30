using FluentValidation;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

public class ScheduleNotificationCommandValidator : AbstractValidator<ScheduleNotificationCommand>
{
    public ScheduleNotificationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.ScheduledAt)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Schedule time must be in the future");
    }
}
