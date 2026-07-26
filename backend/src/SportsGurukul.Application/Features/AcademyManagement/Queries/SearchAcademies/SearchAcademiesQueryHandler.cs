using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.AcademyManagement.Queries.SearchAcademies;

public class SearchAcademiesQueryHandler : IRequestHandler<SearchAcademiesQuery, Result<AcademySearchResponse>>
{
    private readonly IAcademyRepository _academyRepository;
    private readonly ILogger<SearchAcademiesQueryHandler> _logger;

    public SearchAcademiesQueryHandler(
        IAcademyRepository academyRepository,
        ILogger<SearchAcademiesQueryHandler> logger)
    {
        _academyRepository = academyRepository;
        _logger = logger;
    }

    public async Task<Result<AcademySearchResponse>> Handle(SearchAcademiesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Searching academies with filters");

        var allAcademies = await _academyRepository.GetAllAsync(cancellationToken);

        var query = allAcademies.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLowerInvariant();
            query = query.Where(a =>
                a.Name.ToLowerInvariant().Contains(searchTerm) ||
                a.AcademyCode.ToLowerInvariant().Contains(searchTerm) ||
                (a.Description != null && a.Description.ToLowerInvariant().Contains(searchTerm)) ||
                a.Email.ToLowerInvariant().Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            query = query.Where(a => a.Name.ToLowerInvariant().Contains(request.Name.ToLowerInvariant()));
        }

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            query = query.Where(a => a.Contact != null && a.Contact.City != null &&
                a.Contact.City.ToLowerInvariant().Contains(request.City.ToLowerInvariant()));
        }

        if (!string.IsNullOrWhiteSpace(request.State))
        {
            query = query.Where(a => a.Contact != null && a.Contact.State != null &&
                a.Contact.State.ToLowerInvariant().Contains(request.State.ToLowerInvariant()));
        }

        if (!string.IsNullOrWhiteSpace(request.VerificationStatus) &&
            Enum.TryParse<VerificationStatus>(request.VerificationStatus, true, out var verificationStatus))
        {
            query = query.Where(a => a.VerificationStatus == verificationStatus);
        }

        var totalCount = query.Count();
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var pagedAcademies = query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var items = pagedAcademies.Select(a => new AcademySummaryDto
        {
            Id = a.Id,
            AcademyCode = a.AcademyCode,
            Name = a.Name,
            Description = a.Description,
            LogoUrl = a.LogoUrl,
            Email = a.Email,
            Phone = a.Phone,
            Status = a.Status.ToString(),
            VerificationStatus = a.VerificationStatus.ToString(),
            City = a.Contact?.City,
            State = a.Contact?.State,
            Country = a.Contact?.Country,
            TotalBranches = a.Branches?.Count ?? 0,
            TotalFacilities = a.Facilities?.Count ?? 0,
            TotalSports = a.AcademySports?.Count ?? 0,
            TotalMemberships = a.Memberships?.Count ?? 0,
            IsVerified = a.VerificationStatus == VerificationStatus.Verified,
            CreatedAt = a.CreatedAt
        }).ToList();

        var response = new AcademySearchResponse
        {
            Items = items,
            TotalRecords = totalCount,
            TotalPages = totalPages,
            CurrentPage = request.Page,
            PageSize = request.PageSize
        };

        _logger.LogInformation("Search returned {Count} academies (Page {Page} of {TotalPages})", items.Count, request.Page, totalPages);

        return Result<AcademySearchResponse>.Success(response);
    }
}
