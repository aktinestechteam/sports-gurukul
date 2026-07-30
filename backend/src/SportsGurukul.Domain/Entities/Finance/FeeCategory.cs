using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities.Finance;

public class FeeCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<FeeStructure> FeeStructures { get; set; } = new List<FeeStructure>();
}
