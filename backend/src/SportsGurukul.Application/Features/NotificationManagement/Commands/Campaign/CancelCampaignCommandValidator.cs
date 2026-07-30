using FluentValidation;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Campaign;

public class CancelCampaignCommandValidator : AbstractValidator<CancelCampaignCommand>
{
    public CancelCampaignCommandValidator()
    {
        RuleFor(x => x.CampaignId)
            .NotEmpty();
    }
}
