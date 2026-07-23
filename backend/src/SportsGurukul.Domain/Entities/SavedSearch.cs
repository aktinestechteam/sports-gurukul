using System.Text.Json;
using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class SavedSearch : BaseEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FiltersJson { get; set; } = "{}";
    public int UsageCount { get; set; }

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
