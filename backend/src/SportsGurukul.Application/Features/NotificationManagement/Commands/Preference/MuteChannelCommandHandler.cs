using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Preference;

public class MuteChannelCommandHandler
    : IRequestHandler<MuteChannelCommand, Result<bool>>
{
    private readonly IPreferenceService _preferenceService;

    public MuteChannelCommandHandler(IPreferenceService preferenceService)
    {
        _preferenceService = preferenceService;
    }

    public async Task<Result<bool>> Handle(
        MuteChannelCommand request,
        CancellationToken cancellationToken)
    {
        return await _preferenceService.MuteChannelAsync(request.UserId, request.ChannelType, cancellationToken);
    }
}
