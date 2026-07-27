using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.DTOs;

public class TournamentDto
{
    public Guid Id { get; set; }
    public string TournamentCode { get; set; } = string.Empty;
    public string TournamentName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid AcademyId { get; set; }
    public string? AcademyName { get; set; }
    public Guid SportId { get; set; }
    public string? SportName { get; set; }
    public TournamentType TournamentType { get; set; }
    public TournamentStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime RegistrationOpenDate { get; set; }
    public DateTime RegistrationCloseDate { get; set; }
    public int? MaxParticipants { get; set; }
    public int? MinParticipants { get; set; }
    public decimal? RegistrationFee { get; set; }
    public RegistrationType RegistrationType { get; set; }
    public string? Venue { get; set; }
    public string? Rules { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? Website { get; set; }
    public bool IsPublished { get; set; }
    public int RegisteredCount { get; set; }
    public int MatchCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public IReadOnlyList<TournamentCategoryDto> Categories { get; set; } = [];
    public IReadOnlyList<TournamentVenueDto> Venues { get; set; } = [];
}
