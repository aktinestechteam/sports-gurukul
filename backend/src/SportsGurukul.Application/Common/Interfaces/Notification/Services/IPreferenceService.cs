using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Application.Common.Interfaces.Notification.Services;

public interface IPreferenceService
{
    Task<Result<PreferenceDto>> CreateAsync(CreatePreferenceRequest request, CancellationToken cancellationToken = default);
    Task<Result<PreferenceDto>> UpdateAsync(UpdatePreferenceRequest request, CancellationToken cancellationToken = default);
    Task<Result<List<PreferenceDto>>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> IsChannelEnabledAsync(Guid userId, NotificationChannelType channelType, CancellationToken cancellationToken = default);
    Task<Result<bool>> MuteChannelAsync(Guid userId, NotificationChannelType channelType, CancellationToken cancellationToken = default);
    Task<Result<bool>> UnmuteChannelAsync(Guid userId, NotificationChannelType channelType, CancellationToken cancellationToken = default);
    Task<Result<bool>> SubscribeAsync(SubscribeRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UnsubscribeAsync(UnsubscribeRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> IsInQuietHoursAsync(Guid userId, NotificationChannelType channelType, CancellationToken cancellationToken = default);
}
