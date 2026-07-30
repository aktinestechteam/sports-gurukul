using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Preference;

public class UnmuteChannelCommandHandler
    : IRequestHandler<UnmuteChannelCommand, Result<bool>>
{
    private readonly IPreferenceService _preferenceService;

    public UnmuteChannelCommandHandler(IPreferenceService preferenceService)
    {
        _preferenceService = preferenceService;
    }

    public async Task<Result<bool>> Handle(
        UnmuteChannelCommand request,
        CancellationToken cancellationToken)
    {
        return await _preferenceService.UnmuteChannelAsync(request.UserId, request.ChannelType, cancellationToken);
    }
}
