using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Domain.Entities.Finance;

public class FeeStructure : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public FeeFrequency Frequency { get; set; } = FeeFrequency.OneTime;
    public bool IsActive { get; set; } = true;
    public Guid? SportId { get; set; }
    public Guid? AcademyId { get; set; }
    public Guid? FeeCategoryId { get; set; }

    public Sport? Sport { get; set; }
    public Academy? Academy { get; set; }
    public FeeCategory? FeeCategory { get; set; }
}
