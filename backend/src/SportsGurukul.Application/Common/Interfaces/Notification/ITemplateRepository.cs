using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Application.Common.Interfaces.Notification;

public interface ITemplateRepository : IRepository<NotificationTemplate>
{
    Task<NotificationTemplate?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<NotificationTemplate?> GetWithVersionsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotificationTemplate>> GetByChannelAsync(NotificationChannelType channelType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotificationTemplate>> GetActiveTemplatesAsync(CancellationToken cancellationToken = default);
}
