using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Template;

public class CreateTemplateVersionCommandHandler
    : IRequestHandler<CreateTemplateVersionCommand, Result<TemplateVersionDto>>
{
    private readonly ITemplateService _templateService;

    public CreateTemplateVersionCommandHandler(ITemplateService templateService)
    {
        _templateService = templateService;
    }

    public async Task<Result<TemplateVersionDto>> Handle(
        CreateTemplateVersionCommand request,
        CancellationToken cancellationToken)
    {
        var versionRequest = new CreateTemplateVersionRequest(
            request.TemplateId,
            request.SubjectTemplate,
            request.BodyTemplate,
            request.ChangeNotes
        );

        return await _templateService.CreateVersionAsync(versionRequest, cancellationToken);
    }
}
