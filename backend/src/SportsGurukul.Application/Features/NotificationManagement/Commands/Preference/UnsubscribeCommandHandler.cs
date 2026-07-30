using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Preference;

public class UnsubscribeCommandHandler
    : IRequestHandler<UnsubscribeCommand, Result<bool>>
{
    private readonly IPreferenceService _preferenceService;

    public UnsubscribeCommandHandler(IPreferenceService preferenceService)
    {
        _preferenceService = preferenceService;
    }

    public async Task<Result<bool>> Handle(
        UnsubscribeCommand request,
        CancellationToken cancellationToken)
    {
        var unsubscribeRequest = new UnsubscribeRequest(
            request.UserId,
            request.EntityType,
            request.EntityId,
            request.ChannelType,
            request.EventType
        );

        return await _preferenceService.UnsubscribeAsync(unsubscribeRequest, cancellationToken);
    }
}
