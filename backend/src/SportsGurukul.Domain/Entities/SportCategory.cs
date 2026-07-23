using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class SportCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Sport> Sports { get; set; } = new List<Sport>();
}
