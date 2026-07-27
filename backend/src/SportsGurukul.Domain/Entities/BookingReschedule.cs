using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class BookingReschedule : BaseEntity
{
    public Guid BookingId { get; set; }
    public Guid RequestedById { get; set; }
    public DateTime OriginalDate { get; set; }
    public TimeSpan OriginalStartTime { get; set; }
    public TimeSpan OriginalEndTime { get; set; }
    public DateTime NewDate { get; set; }
    public TimeSpan NewStartTime { get; set; }
    public TimeSpan NewEndTime { get; set; }
    public string? Reason { get; set; }
    public bool IsApproved { get; set; }
    public DateTime? ApprovedOn { get; set; }
    public string? Notes { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Booking Booking { get; set; } = null!;
}
