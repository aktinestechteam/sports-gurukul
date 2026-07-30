using FluentValidation;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

public class CreateNotificationCommandValidator : AbstractValidator<CreateNotificationCommand>
{
    public CreateNotificationCommandValidator()
    {
        RuleFor(x => x.ChannelId)
            .NotEmpty()
            .WithMessage("Channel is required");

        RuleFor(x => x.Subject)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Body)
            .NotEmpty();

        RuleFor(x => x.Priority)
            .IsInEnum()
            .WithMessage("Invalid priority level");

        RuleFor(x => x.Recipients)
            .NotEmpty()
            .WithMessage("At least one recipient is required");

        RuleForEach(x => x.Recipients)
            .ChildRules(recipient =>
            {
                recipient.RuleFor(r => r.DestinationAddress)
                    .NotEmpty()
                    .WithMessage("Destination address is required");
            });

        RuleFor(x => x.ScheduledAt)
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.ScheduledAt.HasValue)
            .WithMessage("Schedule time must be in the future");

        RuleFor(x => x.ExternalId)
            .MaximumLength(200);

        RuleFor(x => x.Metadata)
            .MaximumLength(4000);
    }
}
