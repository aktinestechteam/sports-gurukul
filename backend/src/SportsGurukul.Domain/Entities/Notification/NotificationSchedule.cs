using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Domain.Entities.Notification;

public class NotificationSchedule : BaseEntity
{
    public Guid NotificationId { get; set; }
    public DateTime ScheduledAtUtc { get; set; }
    public string TimeZone { get; set; } = "UTC";
    public string? RecurrenceRule { get; set; }
    public DateTime? RecurrenceEndDate { get; set; }
    public NotificationStatus Status { get; set; }
    public DateTime? ProcessedAt { get; set; }

    public Notification Notification { get; set; } = null!;
}
