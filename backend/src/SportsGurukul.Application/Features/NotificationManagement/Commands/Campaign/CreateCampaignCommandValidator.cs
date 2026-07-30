using FluentValidation;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Campaign;

public class CreateCampaignCommandValidator : AbstractValidator<CreateCampaignCommand>
{
    public CreateCampaignCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.ChannelType)
            .IsInEnum();

        RuleFor(x => x.ScheduledAt)
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.ScheduledAt.HasValue)
            .WithMessage("Campaign schedule time must be in the future");
    }
}
