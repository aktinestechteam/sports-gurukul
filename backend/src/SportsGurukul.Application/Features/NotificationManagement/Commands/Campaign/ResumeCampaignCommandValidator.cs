using FluentValidation;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Campaign;

public class ResumeCampaignCommandValidator : AbstractValidator<ResumeCampaignCommand>
{
    public ResumeCampaignCommandValidator()
    {
        RuleFor(x => x.CampaignId)
            .NotEmpty();
    }
}
