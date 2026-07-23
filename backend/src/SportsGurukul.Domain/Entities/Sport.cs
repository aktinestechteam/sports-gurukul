using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class Sport : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool OlympicSport { get; set; }
    public string? Description { get; set; }
    public Guid SportCategoryId { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public SportCategory SportCategory { get; set; } = null!;
    public ICollection<AthleteSport> AthleteSports { get; set; } = new List<AthleteSport>();
}
