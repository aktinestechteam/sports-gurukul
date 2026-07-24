namespace SportsGurukul.Application.Features.CoachManagement.DTOs;

public class CoachDocumentAuditDto
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public string Action { get; set; } = string.Empty;
    public Guid? PerformedBy { get; set; }
    public DateTime PerformedOn { get; set; }
    public string? IpAddress { get; set; }
    public string? Details { get; set; }
}
