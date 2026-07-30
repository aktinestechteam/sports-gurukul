using FluentValidation;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Preference;

public class UpdatePreferenceCommandValidator : AbstractValidator<UpdatePreferenceCommand>
{
    public UpdatePreferenceCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.ChannelType)
            .IsInEnum();
    }
}
