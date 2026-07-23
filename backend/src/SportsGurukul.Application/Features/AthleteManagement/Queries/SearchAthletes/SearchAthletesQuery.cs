using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.AthleteManagement.Queries.SearchAthletes;

public class SearchAthletesQuery : IRequest<Result<AthleteSearchResponse>>
{
    public string? SearchTerm { get; set; }
    public string? Name { get; set; }
    public string? SportName { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public AthleteLevel? CurrentLevel { get; set; }
    public string? Ranking { get; set; }
    public Gender? Gender { get; set; }
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public int? MinExperience { get; set; }
    public int? MaxExperience { get; set; }
    public AthleteStatus? Status { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
