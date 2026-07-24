namespace SportsGurukul.Application.Features.CoachManagement.DTOs;

public class EducationDto
{
    public Guid Id { get; set; }
    public string Degree { get; set; } = string.Empty;
    public string? Institution { get; set; }
    public string? FieldOfStudy { get; set; }
    public int? YearCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
