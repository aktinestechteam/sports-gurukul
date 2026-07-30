using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Campaign;

public class CancelCampaignCommandHandler
    : IRequestHandler<CancelCampaignCommand, Result<bool>>
{
    private readonly ICampaignService _campaignService;

    public CancelCampaignCommandHandler(ICampaignService campaignService)
    {
        _campaignService = campaignService;
    }

    public async Task<Result<bool>> Handle(
        CancelCampaignCommand request,
        CancellationToken cancellationToken)
    {
        return await _campaignService.CancelAsync(request.CampaignId, cancellationToken);
    }
}
