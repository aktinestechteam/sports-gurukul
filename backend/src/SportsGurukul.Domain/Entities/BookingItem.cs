using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class BookingItem : BaseEntity
{
    public Guid BookingId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? ItemDescription { get; set; }
    public int Quantity { get; set; } = 1;
    public string? Unit { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? TotalPrice { get; set; }
    public string? Notes { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Booking Booking { get; set; } = null!;
}
