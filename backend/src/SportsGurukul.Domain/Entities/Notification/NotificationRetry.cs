using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Domain.Entities.Notification;

public class NotificationRetry : BaseEntity
{
    public Guid DeliveryId { get; set; }
    public int AttemptNumber { get; set; }
    public DateTime AttemptedAt { get; set; }
    public NotificationStatus Status { get; set; }
    public string? FailureReason { get; set; }
    public DateTime? NextAttemptAt { get; set; }
    public bool IsFinal { get; set; }

    public NotificationDelivery Delivery { get; set; } = null!;
}
