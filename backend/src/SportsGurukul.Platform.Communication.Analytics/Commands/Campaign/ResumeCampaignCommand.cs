using MediatR;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Commands.Campaign;

public record ResumeCampaignCommand(Guid Id) : IRequest<ResumeCampaignResult>;

public class ResumeCampaignCommandHandler(ICampaignManagementService service) : IRequestHandler<ResumeCampaignCommand, ResumeCampaignResult>
{
    public Task<ResumeCampaignResult> Handle(ResumeCampaignCommand command, CancellationToken ct)
        => service.ResumeAsync(command.Id, ct);
}
