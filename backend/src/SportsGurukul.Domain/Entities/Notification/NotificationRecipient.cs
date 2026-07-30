using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Domain.Entities.Notification;

public class NotificationRecipient : BaseEntity
{
    public Guid NotificationId { get; set; }
    public Guid? UserId { get; set; }
    public NotificationChannelType ChannelType { get; set; }
    public string DestinationAddress { get; set; } = string.Empty;
    public string? RecipientName { get; set; }
    public NotificationStatus Status { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public string? FailureReason { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAtTimestamp { get; set; }

    public Notification Notification { get; set; } = null!;
    public User? User { get; set; }
}
