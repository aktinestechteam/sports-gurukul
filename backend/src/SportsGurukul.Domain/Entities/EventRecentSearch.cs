using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class EventRecentSearch : BaseEntity
{
    public Guid UserId { get; set; }
    public string SearchTerm { get; set; } = string.Empty;
    public string? SportName { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? EventType { get; set; }
    public string? FiltersJson { get; set; }
    public int ResultCount { get; set; }
    public DateTime SearchedAt { get; set; }
    public byte[] RowVersion { get; set; } = [];
}
