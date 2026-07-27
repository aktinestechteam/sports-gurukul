using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.DTOs;

public class AwardDto
{
    public Guid Id { get; set; }
    public Guid TournamentId { get; set; }
    public TournamentAwardType AwardType { get; set; }
    public string AwardName { get; set; } = string.Empty;
    public Guid? ParticipantId { get; set; }
    public string? ParticipantName { get; set; }
    public Guid? TeamId { get; set; }
    public string? TeamName { get; set; }
    public string? Description { get; set; }
    public decimal? PrizeMoney { get; set; }
    public string? CertificateUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
