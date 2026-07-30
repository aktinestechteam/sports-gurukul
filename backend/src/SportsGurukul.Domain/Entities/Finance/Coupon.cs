using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Domain.Entities.Finance;

public class Coupon : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public DiscountType Type { get; set; }
    public decimal Value { get; set; }
    public int? MaxUsage { get; set; }
    public int CurrentUsage { get; set; }
    public int? MaxUsagePerUser { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public decimal? MaxDiscountAmount { get; set; }

    public ICollection<CouponUsage> Usages { get; set; } = new List<CouponUsage>();
}
