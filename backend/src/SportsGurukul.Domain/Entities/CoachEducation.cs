using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class CoachEducation : BaseEntity
{
    public Guid CoachId { get; set; }
    public string Degree { get; set; } = string.Empty;
    public string? Institution { get; set; }
    public string? FieldOfStudy { get; set; }
    public int? YearCompleted { get; set; }

    public Coach Coach { get; set; } = null!;
}
