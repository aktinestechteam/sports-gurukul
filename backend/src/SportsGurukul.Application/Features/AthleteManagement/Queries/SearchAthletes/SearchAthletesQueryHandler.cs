using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;

namespace SportsGurukul.Application.Features.AthleteManagement.Queries.SearchAthletes;

public class SearchAthletesQueryHandler : IRequestHandler<SearchAthletesQuery, Result<AthleteSearchResponse>>
{
    private readonly IAthleteRepository _athleteRepository;
    private readonly ILogger<SearchAthletesQueryHandler> _logger;

    public SearchAthletesQueryHandler(
        IAthleteRepository athleteRepository,
        ILogger<SearchAthletesQueryHandler> logger)
    {
        _athleteRepository = athleteRepository;
        _logger = logger;
    }

    public async Task<Result<AthleteSearchResponse>> Handle(SearchAthletesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Searching athletes with filters");

        var searchRequest = new AthleteSearchRequest
        {
            SearchTerm = request.SearchTerm,
            Name = request.Name,
            SportName = request.SportName,
            City = request.City,
            State = request.State,
            Country = request.Country,
            CurrentLevel = request.CurrentLevel,
            Ranking = request.Ranking,
            Gender = request.Gender,
            MinAge = request.MinAge,
            MaxAge = request.MaxAge,
            MinExperience = request.MinExperience,
            MaxExperience = request.MaxExperience,
            Status = request.Status,
            IsDeleted = request.IsDeleted,
            CreatedFrom = request.CreatedFrom,
            CreatedTo = request.CreatedTo,
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
