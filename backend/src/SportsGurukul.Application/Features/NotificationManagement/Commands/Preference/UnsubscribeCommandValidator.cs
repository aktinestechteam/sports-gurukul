using FluentValidation;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Preference;

public class UnsubscribeCommandValidator : AbstractValidator<UnsubscribeCommand>
{
    public UnsubscribeCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.EntityType)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.EntityId)
            .NotEmpty();

        RuleFor(x => x.ChannelType)
            .IsInEnum();

        RuleFor(x => x.EventType)
            .NotEmpty()
            .MaximumLength(100);
    }
}
