using MediatR;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Commands.Template;

public record RollbackTemplateCommand(
    Guid Id,
    RollbackTemplateRequest Request
) : IRequest<TemplateDetailDto>;

public class RollbackTemplateCommandHandler(ITemplateManagementService service) : IRequestHandler<RollbackTemplateCommand, TemplateDetailDto>
{
    public Task<TemplateDetailDto> Handle(RollbackTemplateCommand command, CancellationToken ct)
        => service.RollbackAsync(command.Id, command.Request, ct);
}
