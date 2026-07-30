using FluentValidation;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Preference;

public class CreatePreferenceCommandValidator : AbstractValidator<CreatePreferenceCommand>
{
    public CreatePreferenceCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.ChannelType)
            .IsInEnum()
            .WithMessage("Invalid notification channel type");

        RuleFor(x => x.MaxPerDay)
            .GreaterThan(0)
            .When(x => x.MaxPerDay.HasValue)
            .WithMessage("Max per day must be greater than 0");
    }
}
