using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Template;

public class PublishTemplateCommandHandler
    : IRequestHandler<PublishTemplateCommand, Result<TemplateDto>>
{
    private readonly ITemplateService _templateService;

    public PublishTemplateCommandHandler(ITemplateService templateService)
    {
        _templateService = templateService;
    }

    public async Task<Result<TemplateDto>> Handle(
        PublishTemplateCommand request,
        CancellationToken cancellationToken)
    {
        return await _templateService.PublishAsync(request.Id, cancellationToken);
    }
}
