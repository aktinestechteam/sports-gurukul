using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities.Notification;

public class NotificationAudit : BaseEntity
{
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public Guid? ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}
