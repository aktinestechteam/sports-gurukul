using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class RecentAcademySearch : BaseEntity
{
    public Guid UserId { get; set; }
    public string SearchTerm { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? State { get; set; }
    public string? SportName { get; set; }
    public int AcademyCount { get; set; }
    public DateTime SearchedAt { get; set; }
}
