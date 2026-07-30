using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Preference;

public class CreatePreferenceCommandHandler
    : IRequestHandler<CreatePreferenceCommand, Result<PreferenceDto>>
{
    private readonly IPreferenceService _preferenceService;

    public CreatePreferenceCommandHandler(IPreferenceService preferenceService)
    {
        _preferenceService = preferenceService;
    }

    public async Task<Result<PreferenceDto>> Handle(
        CreatePreferenceCommand request,
        CancellationToken cancellationToken)
    {
        var createRequest = new CreatePreferenceRequest(
            request.UserId,
            request.ChannelType,
            request.IsEnabled,
            request.QuietHoursStart,
            request.QuietHoursEnd,
            request.MaxPerDay
        );

        return await _preferenceService.CreateAsync(createRequest, cancellationToken);
    }
}
