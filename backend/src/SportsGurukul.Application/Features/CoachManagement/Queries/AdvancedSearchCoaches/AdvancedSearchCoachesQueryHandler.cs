using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.AdvancedSearchCoaches;

public class AdvancedSearchCoachesQueryHandler : IRequestHandler<AdvancedSearchCoachesQuery, Result<AdvancedCoachSearchResponse>>
{
    private readonly ICoachSearchRepository _searchRepository;
    private readonly ILogger<AdvancedSearchCoachesQueryHandler> _logger;

    public AdvancedSearchCoachesQueryHandler(
        ICoachSearchRepository searchRepository,
        ILogger<AdvancedSearchCoachesQueryHandler> logger)
    {
        _searchRepository = searchRepository;
        _logger = logger;
    }

    public async Task<Result<AdvancedCoachSearchResponse>> Handle(AdvancedSearchCoachesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Advanced coach search: Page={Page}, PageSize={PageSize}", request.Page, request.PageSize);

        var (coaches, totalCount) = await _searchRepository.SearchCoachesAsync(
            request.SearchTerm,
            request.Name,
            request.CoachCode,
            request.Email,
            request.Mobile,
            request.SportName,
            request.SportIds,
            request.SportCategory,
            request.CoachingLevel,
            request.MinExperience,
            request.MaxExperience,
            request.CertificationName,
            request.CertificationStatus,
            request.CurrentOrganization,
            request.HighestQualification,
            request.Country,
            request.State,
            request.City,
            request.District,
            request.Latitude,
            request.Longitude,
            request.RadiusKm,
            request.AvailableToday,
            request.OnlineAvailable,
            request.OfflineAvailable,
            request.IsVerified,
            request.BackgroundVerified,
            request.Language,
            request.SortBy,
            request.SortDescending,
            request.Page,
            request.PageSize,
            request.Cursor,
            request.UseCursorPagination,
            cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);
        var items = coaches.Select(MapToSummaryDto).ToList();

        string? nextCursor = null;
        if (request.UseCursorPagination && coaches.Count == request.PageSize)
        {
            var lastCoach = coaches.Last();
            nextCursor = JsonSerializer.Serialize(lastCoach.CreatedAt);
        }

        var response = new AdvancedCoachSearchResponse
        {
            Items = items,
            TotalRecords = totalCount,
            TotalPages = totalPages,
            CurrentPage = request.Page,
            PageSize = request.PageSize,
            NextCursor = nextCursor
        };

        _logger.LogInformation("Advanced search returned {Count} coaches (Page {Page} of {TotalPages})", items.Count, request.Page, totalPages);

        return Result<AdvancedCoachSearchResponse>.Success(response);
    }

    private static CoachSummaryDto MapToSummaryDto(Domain.Entities.Coach coach)
    {
        var primarySport = coach.CoachSports?.FirstOrDefault(cs => cs.IsPrimarySport);

        return new CoachSummaryDto
        {
            Id = coach.Id,
            UserId = coach.UserId,
            CoachCode = coach.CoachCode,
            FullName = coach.User?.FullName ?? string.Empty,
            Email = coach.User?.Email ?? string.Empty,
            PhoneNumber = coach.User?.PhoneNumber,
            ProfileImageUrl = coach.User?.ProfileImageUrl,
            CoachingLevel = coach.CoachingLevel.ToString(),
            Status = coach.Status.ToString(),
            VerificationStatus = coach.VerificationStatus.ToString(),
            PrimarySport = primarySport?.Sport?.Name,
            SportCategory = primarySport?.Sport?.SportCategory?.Name,
            YearsOfExperience = coach.YearsOfExperience,
            City = coach.Location?.City,
            State = coach.Location?.State,
            Country = coach.Location?.Country,
            IsVerified = coach.VerificationStatus == VerificationStatus.Verified,
            CertificationCount = coach.Certifications?.Count ?? 0,
            IsOnlineAvailable = coach.Availability?.OnlineAvailable ?? false,
            IsOfflineAvailable = coach.Availability?.OfflineAvailable ?? false,
            CreatedAt = coach.CreatedAt
        };
    }
}
