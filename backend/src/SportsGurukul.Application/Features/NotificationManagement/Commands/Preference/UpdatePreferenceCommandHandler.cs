using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Preference;

public class UpdatePreferenceCommandHandler
    : IRequestHandler<UpdatePreferenceCommand, Result<PreferenceDto>>
{
    private readonly IPreferenceService _preferenceService;

    public UpdatePreferenceCommandHandler(IPreferenceService preferenceService)
    {
        _preferenceService = preferenceService;
    }

    public async Task<Result<PreferenceDto>> Handle(
        UpdatePreferenceCommand request,
        CancellationToken cancellationToken)
    {
        var updateRequest = new UpdatePreferenceRequest(
            request.UserId,
            request.ChannelType,
            request.IsEnabled,
            request.QuietHoursStart,
            request.QuietHoursEnd,
            request.MaxPerDay
        );

        return await _preferenceService.UpdateAsync(updateRequest, cancellationToken);
    }
}
