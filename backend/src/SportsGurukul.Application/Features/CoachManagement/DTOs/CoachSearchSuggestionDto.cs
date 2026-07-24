namespace SportsGurukul.Application.Features.CoachManagement.DTOs;

public class CoachSearchSuggestionDto
{
    public string Text { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public Guid? Id { get; set; }
    public string? SubText { get; set; }
}
