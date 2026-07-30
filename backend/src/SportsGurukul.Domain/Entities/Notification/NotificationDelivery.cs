using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Domain.Entities.Notification;

public class NotificationDelivery : BaseEntity
{
    public Guid NotificationId { get; set; }
    public Guid? RecipientId { get; set; }
    public Guid? ProviderId { get; set; }
    public NotificationChannelType ChannelType { get; set; }
    public NotificationStatus Status { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public string? FailureReason { get; set; }
    public string? ProviderMessageId { get; set; }
    public string? ProviderResponse { get; set; }
    public int AttemptCount { get; set; }
    public long? DurationMs { get; set; }

    public Notification Notification { get; set; } = null!;
    public NotificationProvider? Provider { get; set; }
    public ICollection<NotificationRetry> Retries { get; set; } = new List<NotificationRetry>();
}
