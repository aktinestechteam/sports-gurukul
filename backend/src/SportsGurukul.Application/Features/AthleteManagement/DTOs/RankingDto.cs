namespace SportsGurukul.Application.Features.AthleteManagement.DTOs;

public class RankingDto
{
    public Guid Id { get; set; }
    public string? CurrentRank { get; set; }
    public string? StateRank { get; set; }
    public string? NationalRank { get; set; }
    public string? InternationalRank { get; set; }
    public string? RankingAuthority { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
