using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.DTOs;

public class ResultDto
{
    public Guid Id { get; set; }
    public Guid TournamentId { get; set; }
    public Guid MatchId { get; set; }
    public int MatchNumber { get; set; }
    public Guid? WinnerId { get; set; }
    public string? WinnerName { get; set; }
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
    public string? HomeParticipantName { get; set; }
    public string? AwayParticipantName { get; set; }
    public string? ResultDetails { get; set; }
    public bool IsVerified { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
