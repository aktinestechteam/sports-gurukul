using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.SearchCoaches;

public class SearchCoachesQueryHandler : IRequestHandler<SearchCoachesQuery, Result<CoachSearchResponse>>
{
    private readonly ICoachRepository _coachRepository;
    private readonly ILogger<SearchCoachesQueryHandler> _logger;

    public SearchCoachesQueryHandler(
        ICoachRepository coachRepository,
        ILogger<SearchCoachesQueryHandler> logger)
    {
        _coachRepository = coachRepository;
        _logger = logger;
    }

    public async Task<Result<CoachSearchResponse>> Handle(SearchCoachesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Searching coaches with filters");

        var coaches = await _coachRepository.GetAllAsync(cancellationToken);

        var query = coaches.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLowerInvariant();
            query = query.Where(c =>
                (c.User.FullName != null && c.User.FullName.ToLowerInvariant().Contains(term)) ||
                (c.User.Email != null && c.User.Email.ToLowerInvariant().Contains(term)) ||
                (c.CoachCode != null && c.CoachCode.ToLowerInvariant().Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var name = request.Name.ToLowerInvariant();
            query = query.Where(c => c.User.FullName != null && c.User.FullName.ToLowerInvariant().Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(request.SportName))
        {
            var sportName = request.SportName.ToLowerInvariant();
            query = query.Where(c => c.CoachSports.Any(cs =>
                cs.Sport.Name.ToLowerInvariant().Contains(sportName)));
        }

        if (!string.IsNullOrWhiteSpace(request.CertificationName))
        {
            var certName = request.CertificationName.ToLowerInvariant();
            query = query.Where(c => c.Certifications.Any(cert =>
                cert.CertificationName.ToLowerInvariant().Contains(certName)));
        }

        if (request.MinExperience.HasValue)
        {
            query = query.Where(c => c.YearsOfExperience >= request.MinExperience.Value);
        }

        if (request.MaxExperience.HasValue)
        {
            query = query.Where(c => c.YearsOfExperience <= request.MaxExperience.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            var city = request.City.ToLowerInvariant();
            query = query.Where(c => c.Location != null && c.Location.City != null &&
                c.Location.City.ToLowerInvariant().Contains(city));
        }

        if (!string.IsNullOrWhiteSpace(request.State))
        {
            var state = request.State.ToLowerInvariant();
            query = query.Where(c => c.Location != null && c.Location.State != null &&
                c.Location.State.ToLowerInvariant().Contains(state));
        }

        if (!string.IsNullOrWhiteSpace(request.Country))
        {
            var country = request.Country.ToLowerInvariant();
            query = query.Where(c => c.Location != null && c.Location.Country != null &&
                c.Location.Country.ToLowerInvariant().Contains(country));
        }

        if (!string.IsNullOrWhiteSpace(request.Language))
        {
            var language = request.Language.ToLowerInvariant();
            query = query.Where(c => c.PreferredLanguage != null &&
                c.PreferredLanguage.ToLowerInvariant().Contains(language));
        }

        if (request.OnlineAvailable.HasValue)
        {
            query = query.Where(c => c.Availability != null &&
                c.Availability.OnlineAvailable == request.OnlineAvailable.Value);
        }

        if (request.OfflineAvailable.HasValue)
        {
            query = query.Where(c => c.Availability != null &&
                c.Availability.OfflineAvailable == request.OfflineAvailable.Value);
        }

        if (request.CoachingLevel.HasValue)
        {
            query = query.Where(c => c.CoachingLevel == request.CoachingLevel.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(c => c.Status == request.Status.Value);
        }

        if (request.VerificationStatus.HasValue)
        {
            query = query.Where(c => c.VerificationStatus == request.VerificationStatus.Value);
        }

        if (request.CreatedFrom.HasValue)
        {
            query = query.Where(c => c.CreatedAt >= request.CreatedFrom.Value);
        }

        if (request.CreatedTo.HasValue)
        {
            query = query.Where(c => c.CreatedAt <= request.CreatedTo.Value);
        }

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

        _logger.LogInformation("Search returned {Count} coaches (Page {Page} of {TotalPages})", items.Count, request.Page, totalPages);

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
            IsVerified = coach.VerificationStatus == VerificationStatus.Verified,
            CertificationCount = coach.Certifications?.Count ?? 0,
            IsOnlineAvailable = coach.Availability?.OnlineAvailable ?? false,
            IsOfflineAvailable = coach.Availability?.OfflineAvailable ?? false,
            CreatedAt = coach.CreatedAt
        };
    }
}
