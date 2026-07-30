using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

namespace SportsGurukul.Application.Features.NotificationManagement.Queries;

public class TemplateVersionsQueryHandler
    : IRequestHandler<TemplateVersionsQuery, Result<List<TemplateVersionDto>>>
{
    private readonly ITemplateService _templateService;

    public TemplateVersionsQueryHandler(ITemplateService templateService)
    {
        _templateService = templateService;
    }

    public async Task<Result<List<TemplateVersionDto>>> Handle(
        TemplateVersionsQuery request,
        CancellationToken cancellationToken)
    {
        return await _templateService.GetVersionsAsync(request.TemplateId, cancellationToken);
    }
}
