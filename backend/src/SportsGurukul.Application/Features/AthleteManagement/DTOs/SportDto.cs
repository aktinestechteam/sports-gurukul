namespace SportsGurukul.Application.Features.AthleteManagement.DTOs;

public class SportDto
{
    public Guid Id { get; set; }
    public Guid SportId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? CategoryName { get; set; }
    public bool OlympicSport { get; set; }
    public bool IsPrimarySport { get; set; }
    public DateTime JoinedDate { get; set; }
}
