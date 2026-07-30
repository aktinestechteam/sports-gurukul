using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Application.Common.Interfaces.Notification;

public interface IPreferenceRepository : IRepository<NotificationPreference>
{
    Task<IReadOnlyList<NotificationPreference>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<NotificationPreference?> GetByUserAndChannelAsync(Guid userId, NotificationChannelType channelType, CancellationToken cancellationToken = default);
    Task<bool> IsChannelEnabledAsync(Guid userId, NotificationChannelType channelType, CancellationToken cancellationToken = default);
}
