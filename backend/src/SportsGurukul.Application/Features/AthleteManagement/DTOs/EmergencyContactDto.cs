namespace SportsGurukul.Application.Features.AthleteManagement.DTOs;

public class EmergencyContactDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
