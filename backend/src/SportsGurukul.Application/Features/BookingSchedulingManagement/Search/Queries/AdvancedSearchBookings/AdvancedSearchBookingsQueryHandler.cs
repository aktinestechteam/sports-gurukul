using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Queries.AdvancedSearchBookings;

public class AdvancedSearchBookingsQueryHandler
    : IRequestHandler<AdvancedSearchBookingsQuery, Result<BookingSearchPageResultDto>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<AdvancedSearchBookingsQueryHandler> _logger;

    public AdvancedSearchBookingsQueryHandler(
        IBookingRepository bookingRepository,
        ILogger<AdvancedSearchBookingsQueryHandler> logger)
    {
        _bookingRepository = bookingRepository;
        _logger = logger;
    }

    public async Task<Result<BookingSearchPageResultDto>> Handle(
        AdvancedSearchBookingsQuery request, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();

        _logger.LogInformation(
            "Advanced booking search: Page={Page}, PageSize={PageSize}, SearchTerm={SearchTerm}",
            request.Page, request.PageSize, request.SearchTerm);

        BookingType? bookingType = null;
        if (!string.IsNullOrWhiteSpace(request.BookingType) &&
            Enum.TryParse<BookingType>(request.BookingType, true, out var parsedType))
        {
            bookingType = parsedType;
        }

        BookingStatus? status = null;
        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<BookingStatus>(request.Status, true, out var parsedStatus))
        {
            status = parsedStatus;
        }

        var bookings = await _bookingRepository.SearchAsync(
            request.AcademyId, request.BranchId, bookingType, status,
            request.SearchTerm, request.Page, request.PageSize, cancellationToken);

        var totalCount = await _bookingRepository.CountSearchAsync(
            request.AcademyId, request.BranchId, bookingType, status,
            request.SearchTerm, cancellationToken);

        var filtered = bookings.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.BookingNumber))
            filtered = filtered.Where(b =>
                b.BookingNumber.Contains(request.BookingNumber, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(request.Title))
            filtered = filtered.Where(b =>
                b.Title.Contains(request.Title, StringComparison.OrdinalIgnoreCase));

        if (request.FacilityId.HasValue)
            filtered = filtered.Where(b => b.FacilityId == request.FacilityId);

        if (request.CoachId.HasValue)
            filtered = filtered.Where(b => b.CoachId == request.CoachId);

        if (request.AthleteId.HasValue)
            filtered = filtered.Where(b => b.AthleteId == request.AthleteId);

        if (!string.IsNullOrWhiteSpace(request.ApprovalStatus) &&
            Enum.TryParse<BookingApprovalStatus>(request.ApprovalStatus, true, out var approval))
        {
            filtered = filtered.Where(b => b.ApprovalStatus == approval);
        }

        if (request.DateFrom.HasValue)
            filtered = filtered.Where(b => b.BookingDate >= request.DateFrom.Value);

        if (request.DateTo.HasValue)
            filtered = filtered.Where(b => b.BookingDate <= request.DateTo.Value);

        if (request.StartTimeFrom.HasValue)
            filtered = filtered.Where(b => b.StartTime >= request.StartTimeFrom.Value);

        if (request.StartTimeTo.HasValue)
            filtered = filtered.Where(b => b.StartTime <= request.StartTimeTo.Value);

        filtered = request.SortBy?.ToLowerInvariant() switch
        {
            "date" => request.SortDescending
                ? filtered.OrderByDescending(b => b.BookingDate).ThenByDescending(b => b.StartTime)
                : filtered.OrderBy(b => b.BookingDate).ThenBy(b => b.StartTime),
            "title" => request.SortDescending
                ? filtered.OrderByDescending(b => b.Title)
                : filtered.OrderBy(b => b.Title),
            "type" => request.SortDescending
                ? filtered.OrderByDescending(b => b.BookingType)
                : filtered.OrderBy(b => b.BookingType),
            "status" => request.SortDescending
                ? filtered.OrderByDescending(b => b.Status)
                : filtered.OrderBy(b => b.Status),
            _ => request.SortDescending
                ? filtered.OrderByDescending(b => b.CreatedAt)
                : filtered.OrderBy(b => b.CreatedAt)
        };

        var items = filtered.Select(b => new BookingSearchResultDto
        {
            Id = b.Id,
            BookingNumber = b.BookingNumber,
            BookingType = b.BookingType.ToString(),
            Status = b.Status.ToString(),
            Title = b.Title,
            Description = b.Description,
            AcademyId = b.AcademyId,
            AcademyName = b.Academy?.Name,
            BranchId = b.BranchId,
            FacilityId = b.FacilityId,
            FacilityName = b.Facility?.FacilityName,
            CoachId = b.CoachId,
            CoachName = b.Coach?.User?.FullName,
            AthleteId = b.AthleteId,
            AthleteName = b.Athlete?.User?.FullName,
            BookingDate = b.BookingDate,
            StartTime = b.StartTime,
            EndTime = b.EndTime,
            Duration = b.Duration,
            ApprovalStatus = b.ApprovalStatus.ToString(),
            BookingCreatorId = b.BookingCreatorId,
            ParticipantCount = b.Participants?.Count ?? 0,
            HasConflict = b.Conflicts?.Any(c => !c.IsResolved) ?? false,
            CreatedAt = b.CreatedAt,
            UpdatedAt = b.UpdatedAt
        }).ToList();

        sw.Stop();

        var result = new BookingSearchPageResultDto
        {
            Items = items,
            TotalRecords = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
            CurrentPage = request.Page,
            PageSize = request.PageSize,
            SearchTimeMs = sw.Elapsed.TotalMilliseconds
        };

        _logger.LogInformation(
            "Advanced search found {Count} bookings in {Ms}ms",
            result.TotalRecords, result.SearchTimeMs);

        return Result<BookingSearchPageResultDto>.Success(result);
    }
}
