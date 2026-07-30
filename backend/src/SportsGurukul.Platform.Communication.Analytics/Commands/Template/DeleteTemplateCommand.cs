using MediatR;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;

namespace SportsGurukul.Platform.Communication.Analytics.Commands.Template;

public record DeleteTemplateCommand(Guid Id) : IRequest<bool>;

public class DeleteTemplateCommandHandler(ITemplateManagementService service) : IRequestHandler<DeleteTemplateCommand, bool>
{
    public Task<bool> Handle(DeleteTemplateCommand command, CancellationToken ct)
        => service.DeleteAsync(command.Id, ct);
}
