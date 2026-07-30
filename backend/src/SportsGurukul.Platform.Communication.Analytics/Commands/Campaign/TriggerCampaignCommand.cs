using MediatR;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Commands.Campaign;

public enum TriggerMode
{
    Immediate,
    Scheduled
}

public record TriggerCampaignCommand(
    Guid Id,
    TriggerMode Mode
) : IRequest<CampaignTriggerResult>;

public class TriggerCampaignCommandHandler(ICampaignManagementService service) : IRequestHandler<TriggerCampaignCommand, CampaignTriggerResult>
{
    public Task<CampaignTriggerResult> Handle(TriggerCampaignCommand command, CancellationToken ct)
        => command.Mode switch
        {
            TriggerMode.Immediate => service.TriggerNowAsync(command.Id, ct),
            TriggerMode.Scheduled => service.TriggerScheduledAsync(command.Id, ct),
            _ => throw new ArgumentOutOfRangeException(nameof(command.Mode))
        };
}
