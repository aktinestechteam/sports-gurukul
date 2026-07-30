using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Domain.Entities.Notification;

public class Notification : BaseEntity
{
    public Guid? TemplateId { get; set; }
    public Guid ChannelId { get; set; }
    public Guid? ProviderId { get; set; }
    public NotificationPriority Priority { get; set; }
    public NotificationStatus Status { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? SenderId { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public string? FailureReason { get; set; }
    public string? ErrorCode { get; set; }
    public Guid? BatchId { get; set; }
    public Guid? CampaignId { get; set; }
    public string? ExternalId { get; set; }
    public string? Metadata { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public NotificationTemplate? Template { get; set; }
    public NotificationChannel Channel { get; set; } = null!;
    public NotificationProvider? Provider { get; set; }
    public NotificationBatch? Batch { get; set; }
    public NotificationCampaign? Campaign { get; set; }
    public NotificationSchedule? Schedule { get; set; }
    public NotificationQueue? QueueEntry { get; set; }
    public ICollection<NotificationRecipient> Recipients { get; set; } = new List<NotificationRecipient>();
    public ICollection<NotificationDelivery> Deliveries { get; set; } = new List<NotificationDelivery>();
    public ICollection<NotificationAttachment> Attachments { get; set; } = new List<NotificationAttachment>();
}
