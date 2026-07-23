using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.AthleteManagement.Queries.AdvancedSearchAthletes;

public class AdvancedSearchAthletesQuery : IRequest<Result<AthleteSearchResponse>>
{
    public string? SearchTerm { get; set; }
    public string? Name { get; set; }
    public string? AthleteCode { get; set; }
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public string? SportName { get; set; }
    public string? SportCategory { get; set; }
    public bool? IsPrimarySport { get; set; }
    public List<Guid>? SportIds { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? District { get; set; }
    public string? PostalCode { get; set; }
    public AthleteLevel? CurrentLevel { get; set; }
    public string? Ranking { get; set; }
    public string? StateRank { get; set; }
    public string? NationalRank { get; set; }
    public string? InternationalRank { get; set; }
    public Gender? Gender { get; set; }
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public string? MinHeight { get; set; }
    public string? MaxHeight { get; set; }
    public string? MinWeight { get; set; }
    public string? MaxWeight { get; set; }
    public BloodGroup? BloodGroup { get; set; }
    public int? MinExperience { get; set; }
    public int? MaxExperience { get; set; }
    public AthleteStatus? Status { get; set; }
    public bool? IsVerified { get; set; }
    public bool? HasMedicalProfile { get; set; }
    public AchievementLevel? MinAchievementLevel { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Cursor { get; set; }
    public bool UseCursorPagination { get; set; }
}
