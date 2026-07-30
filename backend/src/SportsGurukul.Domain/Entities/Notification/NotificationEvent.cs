using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities.Notification;

public class NotificationEvent : BaseEntity
{
    public string EventType { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string? Payload { get; set; }
    public DateTime OccurredAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string Status { get; set; } = "Pending";
    public string? ErrorMessage { get; set; }
}
