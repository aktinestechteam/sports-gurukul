using System.Text.Json;
using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class RecentSearch : BaseEntity
{
    public Guid UserId { get; set; }
    public string QueryText { get; set; } = string.Empty;
    public string FiltersJson { get; set; } = "{}";
    public int ResultCount { get; set; }
    public DateTime SearchedAt { get; set; }

    public User User { get; set; } = null!;

    public T? GetFilters<T>() where T : class
    {
        return JsonSerializer.Deserialize<T>(FiltersJson);
    }

    public void SetFilters<T>(T filters) where T : class
    {
        FiltersJson = JsonSerializer.Serialize(filters);
    }
}
