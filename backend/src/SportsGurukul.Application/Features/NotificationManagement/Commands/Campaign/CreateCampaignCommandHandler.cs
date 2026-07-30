using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Campaign;

public class CreateCampaignCommandHandler
    : IRequestHandler<CreateCampaignCommand, Result<CampaignDto>>
{
    private readonly ICampaignService _campaignService;

    public CreateCampaignCommandHandler(ICampaignService campaignService)
    {
        _campaignService = campaignService;
    }

    public async Task<Result<CampaignDto>> Handle(
        CreateCampaignCommand request,
        CancellationToken cancellationToken)
    {
        var createRequest = new CreateCampaignRequest(
            request.Name,
            request.Description,
            request.TemplateId,
            request.ChannelType,
            request.ScheduledAt,
            request.TargetCriteria,
            request.Metadata
        );

        return await _campaignService.CreateAsync(createRequest, cancellationToken);
    }
}
