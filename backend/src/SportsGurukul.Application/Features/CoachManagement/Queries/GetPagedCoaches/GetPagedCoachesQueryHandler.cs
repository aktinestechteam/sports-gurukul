using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetPagedCoaches;

public class GetPagedCoachesQueryHandler : IRequestHandler<GetPagedCoachesQuery, Result<CoachSearchResponse>>
{
    private readonly ICoachRepository _coachRepository;
    private readonly ILogger<GetPagedCoachesQueryHandler> _logger;

    public GetPagedCoachesQueryHandler(
        ICoachRepository coachRepository,
        ILogger<GetPagedCoachesQueryHandler> logger)
    {
        _coachRepository = coachRepository;
        _logger = logger;
    }

    public async Task<Result<CoachSearchResponse>> Handle(GetPagedCoachesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching paged coaches: Page {Page}, Size {PageSize}", request.Page, request.PageSize);

        var coaches = await _coachRepository.GetAllAsync(cancellationToken);

        var query = coaches.AsQueryable();

        query = request.SortBy?.ToLowerInvariant() switch
        {
            "name" => request.SortDescending
                ? query.OrderByDescending(c => c.User.FullName)
                : query.OrderBy(c => c.User.FullName),
            "experience" => request.SortDescending
                ? query.OrderByDescending(c => c.YearsOfExperience)
                : query.OrderBy(c => c.YearsOfExperience),
            "coachcode" => request.SortDescending
                ? query.OrderByDescending(c => c.CoachCode)
                : query.OrderBy(c => c.CoachCode),
            "status" => request.SortDescending
                ? query.OrderByDescending(c => c.Status)
                : query.OrderBy(c => c.Status),
            "createdat" => request.SortDescending
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.CreatedAt),
            _ => request.SortDescending
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.CreatedAt)
        };

        var totalRecords = query.Count();
        var totalPages = (int)Math.Ceiling(totalRecords / (double)request.PageSize);

        var pagedCoaches = query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var items = pagedCoaches.Select(MapToSummaryDto).ToList();

        var response = new CoachSearchResponse
        {
            Items = items,
            TotalRecords = totalRecords,
            TotalPages = totalPages,
            CurrentPage = request.Page,
            PageSize = request.PageSize
        };

        _logger.LogInformation("Retrieved {Count} coaches (Page {Page} of {TotalPages})", items.Count, request.Page, totalPages);

        return Result<CoachSearchResponse>.Success(response);
    }

    private static CoachSummaryDto MapToSummaryDto(Domain.Entities.Coach coach)
    {
        var primarySport = coach.CoachSports?
            .FirstOrDefault(cs => cs.IsPrimarySport);

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
            IsVerified = coach.VerificationStatus == Domain.Enums.VerificationStatus.Verified,
            CertificationCount = coach.Certifications?.Count ?? 0,
            IsOnlineAvailable = coach.Availability?.OnlineAvailable ?? false,
            IsOfflineAvailable = coach.Availability?.OfflineAvailable ?? false,
            CreatedAt = coach.CreatedAt
        };
    }
}
