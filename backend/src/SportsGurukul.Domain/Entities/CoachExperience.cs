using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class CoachExperience : BaseEntity
{
    public Guid CoachId { get; set; }
    public string Organization { get; set; } = string.Empty;
    public string? Role { get; set; }
    public string? Sport { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Description { get; set; }

    public Coach Coach { get; set; } = null!;
}
