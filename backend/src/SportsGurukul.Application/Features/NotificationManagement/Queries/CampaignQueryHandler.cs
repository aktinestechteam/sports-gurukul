using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

namespace SportsGurukul.Application.Features.NotificationManagement.Queries;

public class CampaignQueryHandler
    : IRequestHandler<CampaignQuery, Result<CampaignDto>>
{
    private readonly ICampaignService _campaignService;

    public CampaignQueryHandler(ICampaignService campaignService)
    {
        _campaignService = campaignService;
    }

    public async Task<Result<CampaignDto>> Handle(
        CampaignQuery request,
        CancellationToken cancellationToken)
    {
        return await _campaignService.GetByIdAsync(request.Id, cancellationToken);
    }
}
