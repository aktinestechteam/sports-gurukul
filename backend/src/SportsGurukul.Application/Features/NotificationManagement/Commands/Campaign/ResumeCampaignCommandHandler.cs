using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Campaign;

public class ResumeCampaignCommandHandler
    : IRequestHandler<ResumeCampaignCommand, Result<bool>>
{
    private readonly ICampaignService _campaignService;

    public ResumeCampaignCommandHandler(ICampaignService campaignService)
    {
        _campaignService = campaignService;
    }

    public async Task<Result<bool>> Handle(
        ResumeCampaignCommand request,
        CancellationToken cancellationToken)
    {
        return await _campaignService.ResumeAsync(request.CampaignId, cancellationToken);
    }
}
