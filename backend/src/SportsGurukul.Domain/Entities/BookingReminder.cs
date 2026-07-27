using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class BookingReminder : BaseEntity
{
    public Guid BookingId { get; set; }
    public int ReminderMinutesBefore { get; set; }
    public DateTime ScheduledAt { get; set; }
    public bool IsSent { get; set; }
    public DateTime? SentAt { get; set; }
    public string? Channel { get; set; }
    public string? Notes { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Booking Booking { get; set; } = null!;
}
