using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Template;

public class ArchiveTemplateCommandHandler
    : IRequestHandler<ArchiveTemplateCommand, Result<bool>>
{
    private readonly ITemplateService _templateService;

    public ArchiveTemplateCommandHandler(ITemplateService templateService)
    {
        _templateService = templateService;
    }

    public async Task<Result<bool>> Handle(
        ArchiveTemplateCommand request,
        CancellationToken cancellationToken)
    {
        return await _templateService.ArchiveAsync(request.Id, cancellationToken);
    }
}
