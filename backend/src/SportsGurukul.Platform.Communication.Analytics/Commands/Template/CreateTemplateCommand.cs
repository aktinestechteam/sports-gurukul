using MediatR;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Commands.Template;

public record CreateTemplateCommand(
    string Name,
    string? Description,
    NotificationChannelType ChannelType,
    TemplateCategory Category,
    string SubjectTemplate,
    string BodyTemplate,
    List<CreateTemplateVariableRequest>? Variables,
    List<CreateLocalizationRequest>? Localizations,
    List<string>? PartialNames,
    List<CreateAttachmentMetaRequest>? Attachments,
    Dictionary<string, string>? Metadata,
    string? CreatedBy
) : IRequest<TemplateDetailDto>;

public class CreateTemplateCommandHandler(ITemplateManagementService service) : IRequestHandler<CreateTemplateCommand, TemplateDetailDto>
{
    public async Task<TemplateDetailDto> Handle(CreateTemplateCommand command, CancellationToken ct)
    {
        var request = new CreateTemplateFullRequest(
            command.Name,
            command.Description,
            command.ChannelType,
            command.Category,
            command.SubjectTemplate,
            command.BodyTemplate,
            command.Variables,
            command.Localizations,
            command.PartialNames,
            command.Attachments,
            command.Metadata
        );
        return await service.CreateAsync(request, command.CreatedBy, ct);
    }
}
