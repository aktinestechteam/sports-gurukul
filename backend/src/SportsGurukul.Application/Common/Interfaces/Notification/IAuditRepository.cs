using SportsGurukul.Domain.Entities.Notification;

namespace SportsGurukul.Application.Common.Interfaces.Notification;

public interface IAuditRepository : IRepository<NotificationAudit>
{
    Task<IReadOnlyList<NotificationAudit>> GetByEntityAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotificationAudit>> GetByActionAsync(string action, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotificationAudit>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
}
