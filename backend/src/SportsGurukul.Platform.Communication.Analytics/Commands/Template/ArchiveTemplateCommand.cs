using MediatR;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Commands.Template;

public record ArchiveTemplateCommand(Guid Id) : IRequest<TemplateDetailDto>;

public class ArchiveTemplateCommandHandler(ITemplateManagementService service) : IRequestHandler<ArchiveTemplateCommand, TemplateDetailDto>
{
    public Task<TemplateDetailDto> Handle(ArchiveTemplateCommand command, CancellationToken ct)
        => service.ArchiveAsync(command.Id, ct);
}
