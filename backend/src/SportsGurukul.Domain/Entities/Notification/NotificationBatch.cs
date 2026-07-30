using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Domain.Entities.Notification;

public class NotificationBatch : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public NotificationStatus Status { get; set; }
    public int TotalCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Metadata { get; set; }

    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
