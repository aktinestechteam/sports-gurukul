using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class BookingHistory : BaseEntity
{
    public Guid BookingId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? PreviousValue { get; set; }
    public string? NewValue { get; set; }
    public string? PerformedBy { get; set; }
    public DateTime PerformedOn { get; set; }
    public string? Notes { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Booking Booking { get; set; } = null!;
}
