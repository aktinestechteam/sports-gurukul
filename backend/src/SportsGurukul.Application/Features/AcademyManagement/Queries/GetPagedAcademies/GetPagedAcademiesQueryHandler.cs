using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.AcademyManagement.Queries.GetPagedAcademies;

public class GetPagedAcademiesQueryHandler : IRequestHandler<GetPagedAcademiesQuery, Result<AcademySearchResponse>>
{
    private readonly IRepository<Academy> _academyRepository;
    private readonly ILogger<GetPagedAcademiesQueryHandler> _logger;

    public GetPagedAcademiesQueryHandler(
        IRepository<Academy> academyRepository,
        ILogger<GetPagedAcademiesQueryHandler> logger)
    {
        _academyRepository = academyRepository;
        _logger = logger;
    }

    public async Task<Result<AcademySearchResponse>> Handle(GetPagedAcademiesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching paged academies: Page {Page}, Size {PageSize}", request.Page, request.PageSize);

        var allAcademies = await _academyRepository.GetAllAsync(cancellationToken);

        var query = allAcademies.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLowerInvariant();
            query = query.Where(a =>
                a.Name.ToLowerInvariant().Contains(searchTerm) ||
                a.AcademyCode.ToLowerInvariant().Contains(searchTerm) ||
                a.Email.ToLowerInvariant().Contains(searchTerm));
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

        _logger.LogInformation("Retrieved {Count} academies (Page {Page} of {TotalPages})", items.Count, request.Page, totalPages);

        return Result<AcademySearchResponse>.Success(response);
    }
}
