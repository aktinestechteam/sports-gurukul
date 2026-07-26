using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;

namespace SportsGurukul.Application.Features.FacilityManagement.Queries.SearchFacilities;

public class SearchFacilitiesQueryHandler : IRequestHandler<SearchFacilitiesQuery, Result<FacilitySearchResponse>>
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly ILogger<SearchFacilitiesQueryHandler> _logger;

    public SearchFacilitiesQueryHandler(
        IFacilityRepository facilityRepository,
        ILogger<SearchFacilitiesQueryHandler> logger)
    {
        _facilityRepository = facilityRepository;
        _logger = logger;
    }

    public async Task<Result<FacilitySearchResponse>> Handle(SearchFacilitiesQuery request, CancellationToken cancellationToken)
    {
        var facilities = await _facilityRepository.SearchAsync(
            request.AcademyId,
            request.FacilityType,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalRecords = await _facilityRepository.CountSearchAsync(
            request.AcademyId,
            request.FacilityType,
            request.SearchTerm,
            cancellationToken);

        var items = facilities.Select(f => new FacilitySummaryDto
        {
            Id = f.Id,
            AcademyId = f.AcademyId,
            FacilityCode = f.FacilityCode,
            FacilityName = f.FacilityName,
            FacilityType = f.FacilityType.ToString(),
            Capacity = f.Capacity,
            IndoorOutdoor = f.IndoorOutdoor.ToString(),
            Status = f.Status.ToString(),
            TotalCourts = f.Courts.Count,
            CreatedAt = f.CreatedAt
        }).ToList();

        var response = new FacilitySearchResponse
        {
            Items = items,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize),
            CurrentPage = request.Page,
            PageSize = request.PageSize
        };

        _logger.LogInformation("Found {TotalRecords} facilities matching search criteria", totalRecords);

        return Result<FacilitySearchResponse>.Success(response);
    }
}
