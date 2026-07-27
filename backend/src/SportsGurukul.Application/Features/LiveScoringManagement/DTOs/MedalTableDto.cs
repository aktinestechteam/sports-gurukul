namespace SportsGurukul.Application.Features.LiveScoringManagement.DTOs;

public class MedalTableDto
{
    public Guid TournamentId { get; set; }
    public DateTime GeneratedAt { get; set; }
    public List<MedalTableEntryDto> Entries { get; set; } = new();
}

public class MedalTableEntryDto
{
    public Guid ParticipantId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public string? AcademyName { get; set; }
    public int GoldCount { get; set; }
    public int SilverCount { get; set; }
    public int BronzeCount { get; set; }
    public int TotalMedals { get; set; }
    public int TotalPoints { get; set; }
}
