using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class BookingSchedule : BaseEntity
{
    public Guid BookingId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsCancelled { get; set; }
    public string? CancellationReason { get; set; }
    public string? Notes { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Booking Booking { get; set; } = null!;
}
