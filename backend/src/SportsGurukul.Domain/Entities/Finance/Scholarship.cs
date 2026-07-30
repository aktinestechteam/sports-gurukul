using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities.Finance;

public class Scholarship : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public string? Criteria { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
}
