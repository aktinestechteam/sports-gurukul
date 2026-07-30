using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Domain.Entities.Notification;

public class NotificationProvider : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public NotificationChannelType ChannelType { get; set; }
    public Guid ChannelId { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; }
    public int Priority { get; set; }
    public string? Configuration { get; set; }

    public NotificationChannel Channel { get; set; } = null!;
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<NotificationDelivery> Deliveries { get; set; } = new List<NotificationDelivery>();
}
