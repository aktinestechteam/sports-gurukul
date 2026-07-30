using MediatR;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Commands.Template;

public record PublishTemplateCommand(
    Guid Id,
    string? PublishedBy
) : IRequest<TemplateDetailDto>;

public class PublishTemplateCommandHandler(ITemplateManagementService service) : IRequestHandler<PublishTemplateCommand, TemplateDetailDto>
{
    public Task<TemplateDetailDto> Handle(PublishTemplateCommand command, CancellationToken ct)
        => service.PublishAsync(command.Id, command.PublishedBy, ct);
}
