using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Campaign;

public class PauseCampaignCommandHandler
    : IRequestHandler<PauseCampaignCommand, Result<bool>>
{
    private readonly ICampaignService _campaignService;

    public PauseCampaignCommandHandler(ICampaignService campaignService)
    {
        _campaignService = campaignService;
    }

    public async Task<Result<bool>> Handle(
        PauseCampaignCommand request,
        CancellationToken cancellationToken)
    {
        return await _campaignService.PauseAsync(request.CampaignId, cancellationToken);
    }
}
