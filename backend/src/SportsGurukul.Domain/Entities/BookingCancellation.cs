using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class BookingCancellation : BaseEntity
{
    public Guid BookingId { get; set; }
    public Guid CancelledByUserId { get; set; }
    public DateTime CancelledOn { get; set; }
    public string Reason { get; set; } = string.Empty;
    public decimal? RefundAmount { get; set; }
    public bool IsRefundProcessed { get; set; }
    public string? Notes { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Booking Booking { get; set; } = null!;
}
