using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities.Finance;

public class PaymentMethod : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}
