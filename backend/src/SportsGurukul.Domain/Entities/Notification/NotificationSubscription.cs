using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Domain.Entities.Notification;

public class NotificationSubscription : BaseEntity
{
    public Guid UserId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public NotificationChannelType ChannelType { get; set; }
    public string EventType { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public User User { get; set; } = null!;
}
