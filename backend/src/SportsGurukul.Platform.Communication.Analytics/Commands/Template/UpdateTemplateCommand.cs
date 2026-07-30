using MediatR;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Commands.Template;

public record UpdateTemplateCommand(
    Guid Id,
    UpdateTemplateFullRequest Request
) : IRequest<TemplateDetailDto>;

public class UpdateTemplateCommandHandler(ITemplateManagementService service) : IRequestHandler<UpdateTemplateCommand, TemplateDetailDto>
{
    public Task<TemplateDetailDto> Handle(UpdateTemplateCommand command, CancellationToken ct)
        => service.UpdateAsync(command.Id, command.Request, ct);
}
