using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

namespace SportsGurukul.Application.Features.NotificationManagement.Queries;

public class TemplateQueryHandler
    : IRequestHandler<TemplateQuery, Result<TemplateDto>>
{
    private readonly ITemplateService _templateService;

    public TemplateQueryHandler(ITemplateService templateService)
    {
        _templateService = templateService;
    }

    public async Task<Result<TemplateDto>> Handle(
        TemplateQuery request,
        CancellationToken cancellationToken)
    {
        return await _templateService.GetByIdAsync(request.Id, cancellationToken);
    }
}
