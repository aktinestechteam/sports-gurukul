using FluentValidation;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Campaign;

public class ScheduleCampaignCommandValidator : AbstractValidator<ScheduleCampaignCommand>
{
    public ScheduleCampaignCommandValidator()
    {
        RuleFor(x => x.CampaignId)
            .NotEmpty();

        RuleFor(x => x.ScheduledAt)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Schedule time must be in the future");
    }
}
