using MediatR;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Commands.Campaign;

public record PauseCampaignCommand(Guid Id) : IRequest<PauseCampaignResult>;

public class PauseCampaignCommandHandler(ICampaignManagementService service) : IRequestHandler<PauseCampaignCommand, PauseCampaignResult>
{
    public Task<PauseCampaignResult> Handle(PauseCampaignCommand command, CancellationToken ct)
        => service.PauseAsync(command.Id, ct);
}
