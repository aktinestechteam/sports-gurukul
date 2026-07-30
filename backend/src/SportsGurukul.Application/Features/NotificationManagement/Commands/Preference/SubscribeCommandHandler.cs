using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Preference;

public class SubscribeCommandHandler
    : IRequestHandler<SubscribeCommand, Result<bool>>
{
    private readonly IPreferenceService _preferenceService;

    public SubscribeCommandHandler(IPreferenceService preferenceService)
    {
        _preferenceService = preferenceService;
    }

    public async Task<Result<bool>> Handle(
        SubscribeCommand request,
        CancellationToken cancellationToken)
    {
        var subscribeRequest = new SubscribeRequest(
            request.UserId,
            request.EntityType,
            request.EntityId,
            request.ChannelType,
            request.EventType
        );

        return await _preferenceService.SubscribeAsync(subscribeRequest, cancellationToken);
    }
}
