using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class CoachSpecialization : BaseEntity
{
    public Guid CoachId { get; set; }
    public string SpecializationName { get; set; } = string.Empty;
    public string? Description { get; set; }

    public Coach Coach { get; set; } = null!;
}
