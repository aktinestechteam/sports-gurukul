using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

namespace SportsGurukul.Application.Features.NotificationManagement.Queries;

public class PreferenceQueryHandler
    : IRequestHandler<PreferenceQuery, Result<List<PreferenceDto>>>
{
    private readonly IPreferenceService _preferenceService;

    public PreferenceQueryHandler(IPreferenceService preferenceService)
    {
        _preferenceService = preferenceService;
    }

    public async Task<Result<List<PreferenceDto>>> Handle(
        PreferenceQuery request,
        CancellationToken cancellationToken)
    {
        return await _preferenceService.GetByUserAsync(request.UserId, cancellationToken);
    }
}
