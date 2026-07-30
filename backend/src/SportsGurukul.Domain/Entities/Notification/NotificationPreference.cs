using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Domain.Entities.Notification;

public class NotificationPreference : BaseEntity
{
    public Guid UserId { get; set; }
    public NotificationChannelType ChannelType { get; set; }
    public bool IsEnabled { get; set; } = true;
    public TimeOnly? QuietHoursStart { get; set; }
    public TimeOnly? QuietHoursEnd { get; set; }
    public int? MaxPerDay { get; set; }

    public User User { get; set; } = null!;
}
