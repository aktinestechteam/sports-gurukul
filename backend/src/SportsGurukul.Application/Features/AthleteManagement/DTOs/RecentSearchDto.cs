namespace SportsGurukul.Application.Features.AthleteManagement.DTOs;

public class RecentSearchDto
{
    public Guid Id { get; set; }
    public string QueryText { get; set; } = string.Empty;
    public string FiltersJson { get; set; } = "{}";
    public int ResultCount { get; set; }
    public DateTime SearchedAt { get; set; }
}
