using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Template;

public class CreateTemplateCommandHandler
    : IRequestHandler<CreateTemplateCommand, Result<TemplateDto>>
{
    private readonly ITemplateService _templateService;

    public CreateTemplateCommandHandler(ITemplateService templateService)
    {
        _templateService = templateService;
    }

    public async Task<Result<TemplateDto>> Handle(
        CreateTemplateCommand request,
        CancellationToken cancellationToken)
    {
        var createRequest = new CreateTemplateRequest(
            request.Name,
            request.Description,
            request.ChannelType,
            request.SubjectTemplate,
            request.BodyTemplate,
            request.Variables
        );

        return await _templateService.CreateAsync(createRequest, cancellationToken);
    }
}
