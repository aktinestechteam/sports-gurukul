using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class BookingWaitlist : BaseEntity
{
    public Guid BookingId { get; set; }
    public Guid WaitlistUserId { get; set; }
    public int Priority { get; set; }
    public DateTime RequestedOn { get; set; }
    public int PromotionOrder { get; set; }
    public WaitlistStatus Status { get; set; } = WaitlistStatus.Active;
    public string? Notes { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Booking Booking { get; set; } = null!;
}
