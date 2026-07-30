using MediatR;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;

namespace SportsGurukul.Platform.Communication.Analytics.Commands.Campaign;

public record DeleteCampaignCommand(Guid Id) : IRequest<bool>;

public class DeleteCampaignCommandHandler(ICampaignManagementService service) : IRequestHandler<DeleteCampaignCommand, bool>
{
    public Task<bool> Handle(DeleteCampaignCommand command, CancellationToken ct)
        => service.DeleteAsync(command.Id, ct);
}
