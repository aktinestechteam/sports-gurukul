using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Domain.Entities.Notification;

public class NotificationQueue : BaseEntity
{
    public Guid NotificationId { get; set; }
    public NotificationChannelType ChannelType { get; set; }
    public NotificationStatus Status { get; set; }
    public NotificationPriority Priority { get; set; }
    public DateTime QueuedAt { get; set; }
    public DateTime? ProcessStartedAt { get; set; }
    public DateTime? ProcessCompletedAt { get; set; }
    public int AttemptCount { get; set; }
    public int MaxAttempts { get; set; } = 3;
    public DateTime? NextAttemptAt { get; set; }
    public DateTime? LockExpiresAt { get; set; }
    public string? LockToken { get; set; }
    public Guid? BatchId { get; set; }

    public Notification Notification { get; set; } = null!;
}
