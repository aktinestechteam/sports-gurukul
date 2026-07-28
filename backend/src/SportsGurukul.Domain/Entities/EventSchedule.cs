using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class EventSchedule : BaseEntity
{
    public Guid EventId { get; set; }
    public DateTime ScheduleDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool IsAllDay { get; set; }
    public string? RecurrenceRule { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Event Event { get; set; } = null!;
}
