using MediatR;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Commands.Campaign;

public record UpdateCampaignCommand(
    Guid Id,
    UpdateCampaignRequest Request
) : IRequest<CampaignDetailDto>;

public class UpdateCampaignCommandHandler(ICampaignManagementService service) : IRequestHandler<UpdateCampaignCommand, CampaignDetailDto>
{
    public Task<CampaignDetailDto> Handle(UpdateCampaignCommand command, CancellationToken ct)
        => service.UpdateAsync(command.Id, command.Request, ct);
}
