namespace SportsGurukul.Application.Features.AthleteManagement.DTOs;

public class SavedSearchDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FiltersJson { get; set; } = "{}";
    public int UsageCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
