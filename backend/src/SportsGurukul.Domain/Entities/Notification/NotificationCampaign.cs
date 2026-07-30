using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Domain.Entities.Notification;

public class NotificationCampaign : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? TemplateId { get; set; }
    public NotificationChannelType ChannelType { get; set; }
    public NotificationStatus Status { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? TargetCriteria { get; set; }
    public int TotalCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public string? Metadata { get; set; }

    public NotificationTemplate? Template { get; set; }
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
