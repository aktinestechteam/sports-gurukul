using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Template;

public class UpdateTemplateCommandHandler
    : IRequestHandler<UpdateTemplateCommand, Result<TemplateDto>>
{
    private readonly ITemplateService _templateService;

    public UpdateTemplateCommandHandler(ITemplateService templateService)
    {
        _templateService = templateService;
    }

    public async Task<Result<TemplateDto>> Handle(
        UpdateTemplateCommand request,
        CancellationToken cancellationToken)
    {
        var updateRequest = new UpdateTemplateRequest(
            request.Id,
            request.Name,
            request.Description,
            request.SubjectTemplate,
            request.BodyTemplate,
            request.Variables
        );

        return await _templateService.UpdateAsync(updateRequest, cancellationToken);
    }
}
