using MediatR;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Commands.Campaign;

public record CancelCampaignCommand(Guid Id) : IRequest<CampaignDetailDto>;

public class CancelCampaignCommandHandler(ICampaignManagementService service) : IRequestHandler<CancelCampaignCommand, CampaignDetailDto>
{
    public Task<CampaignDetailDto> Handle(CancelCampaignCommand command, CancellationToken ct)
        => service.CancelAsync(command.Id, ct);
}
