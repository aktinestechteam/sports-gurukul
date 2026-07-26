using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Queries.SearchAcademies;

public class SearchAcademiesQuery : IRequest<Result<AcademySearchResponse>>
{
    public string? SearchTerm { get; set; }
    public string? Name { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? SportName { get; set; }
    public string? VerificationStatus { get; set; }
    public string? MembershipType { get; set; }
    public string? FacilityType { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
