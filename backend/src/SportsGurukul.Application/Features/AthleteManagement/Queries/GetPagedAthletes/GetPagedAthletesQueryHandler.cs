using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;

namespace SportsGurukul.Application.Features.AthleteManagement.Queries.GetPagedAthletes;

public class GetPagedAthletesQueryHandler : IRequestHandler<GetPagedAthletesQuery, Result<AthleteSearchResponse>>
{
    private readonly IAthleteRepository _athleteRepository;
    private readonly ILogger<GetPagedAthletesQueryHandler> _logger;

    public GetPagedAthletesQueryHandler(
        IAthleteRepository athleteRepository,
        ILogger<GetPagedAthletesQueryHandler> logger)
    {
        _athleteRepository = athleteRepository;
        _logger = logger;
    }

    public async Task<Result<AthleteSearchResponse>> Handle(GetPagedAthletesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching paged athletes: Page {Page}, Size {PageSize}", request.Page, request.PageSize);

        var searchRequest = new AthleteSearchRequest
        {
            SortBy = request.SortBy,
            SortDescending = request.SortDescending,
            Page = request.Page,
            PageSize = request.PageSize
        };

        var (athletes, totalCount) = await _athleteRepository.SearchAthletesAsync(searchRequest, cancellationToken);

        var response = new AthleteSearchResponse
        {
            Items = athletes,
            TotalRecords = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
            CurrentPage = request.Page,
            PageSize = request.PageSize
        };

        return Result<AthleteSearchResponse>.Success(response);
    }
}
