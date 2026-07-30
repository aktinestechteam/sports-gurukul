using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Domain.Entities.Notification;

public class NotificationChannel : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public NotificationChannelType ChannelType { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }

    public ICollection<NotificationProvider> Providers { get; set; } = new List<NotificationProvider>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
