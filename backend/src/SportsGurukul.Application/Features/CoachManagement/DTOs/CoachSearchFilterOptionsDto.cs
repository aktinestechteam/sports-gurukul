namespace SportsGurukul.Application.Features.CoachManagement.DTOs;

public class CoachSearchFilterOptionsDto
{
    public IReadOnlyList<string> Sports { get; set; } = [];
    public IReadOnlyList<string> SportCategories { get; set; } = [];
    public IReadOnlyList<string> CoachingLevels { get; set; } = [];
    public IReadOnlyList<string> Countries { get; set; } = [];
    public IReadOnlyList<string> States { get; set; } = [];
    public IReadOnlyList<string> Languages { get; set; } = [];
    public IReadOnlyList<string> CertificationTypes { get; set; } = [];
    public IReadOnlyList<string> Organizations { get; set; } = [];
}
