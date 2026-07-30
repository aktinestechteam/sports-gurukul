using MediatR;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Commands.Campaign;

public record CloneCampaignCommand(
    Guid Id,
    CampaignCloneRequest Request
) : IRequest<CampaignDetailDto>;

public class CloneCampaignCommandHandler(ICampaignManagementService service) : IRequestHandler<CloneCampaignCommand, CampaignDetailDto>
{
    public Task<CampaignDetailDto> Handle(CloneCampaignCommand command, CancellationToken ct)
        => service.CloneAsync(command.Id, command.Request, ct);
}
