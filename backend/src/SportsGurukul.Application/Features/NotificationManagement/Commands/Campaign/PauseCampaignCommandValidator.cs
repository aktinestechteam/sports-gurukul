using FluentValidation;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Campaign;

public class PauseCampaignCommandValidator : AbstractValidator<PauseCampaignCommand>
{
    public PauseCampaignCommandValidator()
    {
        RuleFor(x => x.CampaignId)
            .NotEmpty();
    }
}
