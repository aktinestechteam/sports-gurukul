using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Domain.Entities.Finance;

public class DiscountPolicy : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal? MaxAmount { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
}
