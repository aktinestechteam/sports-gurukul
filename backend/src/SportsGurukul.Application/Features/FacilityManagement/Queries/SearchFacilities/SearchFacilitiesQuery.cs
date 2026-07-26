using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.FacilityManagement.Queries.SearchFacilities;

public class SearchFacilitiesQuery : IRequest<Result<FacilitySearchResponse>>
{
    public Guid? AcademyId { get; set; }
    public FacilityType? FacilityType { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
