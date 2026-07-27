using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.DTOs;

public class TournamentSummaryDto
{
    public Guid Id { get; set; }
    public string TournamentCode { get; set; } = string.Empty;
    public string TournamentName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? AcademyName { get; set; }
    public string? SportName { get; set; }
    public TournamentType TournamentType { get; set; }
    public TournamentStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? MaxParticipants { get; set; }
    public int RegisteredCount { get; set; }
    public decimal? RegistrationFee { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TournamentSearchResponse
{
    public IReadOnlyList<TournamentSummaryDto> Items { get; set; } = [];
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
}

public class TournamentCategoryDto
{
    public Guid Id { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public TournamentCategoryType CategoryType { get; set; }
    public string? Description { get; set; }
    public int MinAge { get; set; }
    public int MaxAge { get; set; }
    public Gender? Gender { get; set; }
    public bool IsActive { get; set; }
}

public class TournamentVenueDto
{
    public Guid Id { get; set; }
    public Guid FacilityId { get; set; }
    public string? VenueName { get; set; }
    public string? Address { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsActive { get; set; }
}
