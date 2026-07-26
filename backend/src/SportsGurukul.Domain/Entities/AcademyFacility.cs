using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class AcademyFacility : BaseEntity
{
    public Guid AcademyId { get; set; }
    public string FacilityName { get; set; } = string.Empty;
    public AcademyFacilityType FacilityType { get; set; }
    public string? IndoorOutdoor { get; set; }
    public int? Capacity { get; set; }
    public bool Available { get; set; } = true;
    public string? Description { get; set; }

    public Academy Academy { get; set; } = null!;
}
