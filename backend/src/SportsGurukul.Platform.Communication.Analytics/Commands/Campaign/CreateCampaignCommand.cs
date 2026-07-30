using MediatR;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Commands.Campaign;

public record CreateCampaignCommand(
    string Name,
    string? Description,
    CampaignType CampaignType,
    Guid? TemplateId,
    NotificationChannelType ChannelType,
    ScheduleDefinitionDto? Schedule,
    AudienceDefinitionDto? Audience,
    string? Metadata,
    string? CreatedBy
) : IRequest<CampaignDetailDto>;

public class CreateCampaignCommandHandler(ICampaignManagementService service) : IRequestHandler<CreateCampaignCommand, CampaignDetailDto>
{
    public async Task<CampaignDetailDto> Handle(CreateCampaignCommand command, CancellationToken ct)
    {
        var request = new CreateCampaignFullRequest(
            command.Name,
            command.Description,
            command.CampaignType,
            command.TemplateId,
            command.ChannelType,
            command.Schedule,
            command.Audience,
            command.Metadata
        );
        return await service.CreateAsync(request, command.CreatedBy, ct);
    }
}
