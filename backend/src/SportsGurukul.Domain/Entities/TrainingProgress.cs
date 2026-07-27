using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class TrainingProgress : BaseEntity
{
    public Guid EnrollmentId { get; set; }
    public string CurrentLevel { get; set; } = string.Empty;
    public decimal CompletedPercentage { get; set; }
    public decimal? OverallRating { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public TrainingEnrollment Enrollment { get; set; } = null!;
}
