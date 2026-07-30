using MediatR;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Commands.Template;

public record CloneTemplateCommand(
    Guid Id,
    CloneTemplateRequest Request
) : IRequest<TemplateDetailDto>;

public class CloneTemplateCommandHandler(ITemplateManagementService service) : IRequestHandler<CloneTemplateCommand, TemplateDetailDto>
{
    public Task<TemplateDetailDto> Handle(CloneTemplateCommand command, CancellationToken ct)
        => service.CloneAsync(command.Id, command.Request, ct);
}
