using FluentValidation;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Preference;

public class MuteChannelCommandValidator : AbstractValidator<MuteChannelCommand>
{
    public MuteChannelCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.ChannelType)
            .IsInEnum();
    }
}
