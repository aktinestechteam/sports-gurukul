using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities.Finance;

public class CouponUsage : BaseEntity
{
    public Guid CouponId { get; set; }
    public Guid UserId { get; set; }
    public Guid? OrderId { get; set; }
    public DateTime UsedAt { get; set; }
    public decimal DiscountAmount { get; set; }

    public Coupon Coupon { get; set; } = null!;
    public User User { get; set; } = null!;
}
