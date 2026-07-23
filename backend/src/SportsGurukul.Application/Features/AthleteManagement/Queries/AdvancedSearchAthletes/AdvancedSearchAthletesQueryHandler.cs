using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;

namespace SportsGurukul.Application.Features.AthleteManagement.Queries.AdvancedSearchAthletes;

public class AdvancedSearchAthletesQueryHandler : IRequestHandler<AdvancedSearchAthletesQuery, Result<AthleteSearchResponse>>
{
    private readonly IAthleteRepository _athleteRepository;
    private readonly ILogger<AdvancedSearchAthletesQueryHandler> _logger;

    public AdvancedSearchAthletesQueryHandler(
        IAthleteRepository athleteRepository,
        ILogger<AdvancedSearchAthletesQueryHandler> logger)
    {
        _athleteRepository = athleteRepository;
        _logger = logger;
    }

    public async Task<Result<AthleteSearchResponse>> Handle(AdvancedSearchAthletesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Advanced athlete search requested: Page={Page}, PageSize={PageSize}", request.Page, request.PageSize);

        var searchRequest = new AthleteSearchRequest
        {
            SearchTerm = request.SearchTerm,
            Name = request.Name,
            AthleteCode = request.AthleteCode,
            Email = request.Email,
            Mobile = request.Mobile,
            SportName = request.SportName,
            SportCategory = request.SportCategory,
            IsPrimarySport = request.IsPrimarySport,
            SportIds = request.SportIds,
            City = request.City,
            State = request.State,
            Country = request.Country,
            District = request.District,
            PostalCode = request.PostalCode,
            CurrentLevel = request.CurrentLevel,
            Ranking = request.Ranking,
            StateRank = request.StateRank,
            NationalRank = request.NationalRank,
            InternationalRank = request.InternationalRank,
            Gender = request.Gender,
            MinAge = request.MinAge,
            MaxAge = request.MaxAge,
            MinHeight = request.MinHeight,
            MaxHeight = request.MaxHeight,
            MinWeight = request.MinWeight,
            MaxWeight = request.MaxWeight,
            BloodGroup = request.BloodGroup,
            MinExperience = request.MinExperience,
            MaxExperience = request.MaxExperience,
            Status = request.Status,
            IsVerified = request.IsVerified,
            HasMedicalProfile = request.HasMedicalProfile,
            MinAchievementLevel = request.MinAchievementLevel,
            CreatedFrom = request.CreatedFrom,
            CreatedTo = request.CreatedTo,
            SortBy = request.SortBy,
            SortDescending = request.SortDescending,
            Page = request.Page,
            PageSize = request.PageSize,
            Cursor = request.Cursor,
            UseCursorPagination = request.UseCursorPagination
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

        if (athletes.Count > 0)
        {
            response.PreviousCursor = request.Page > 1
                ? Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(athletes.First().Id.ToString()))
                : null;
            response.NextCursor = athletes.Count == request.PageSize
                ? Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(athletes.Last().Id.ToString()))
                : null;
        }

        return Result<AthleteSearchResponse>.Success(response);
    }
}
