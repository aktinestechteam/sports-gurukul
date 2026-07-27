using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class SessionSchedule : BaseEntity
{
    public Guid SessionId { get; set; }
    public int DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsRecurring { get; set; }
    public string? Notes { get; set; }

    public TrainingSession Session { get; set; } = null!;
}
