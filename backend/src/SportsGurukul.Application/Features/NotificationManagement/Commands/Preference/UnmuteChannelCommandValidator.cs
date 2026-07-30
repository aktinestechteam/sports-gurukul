using FluentValidation;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Preference;

public class UnmuteChannelCommandValidator : AbstractValidator<UnmuteChannelCommand>
{
    public UnmuteChannelCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.ChannelType)
            .IsInEnum();
    }
}
