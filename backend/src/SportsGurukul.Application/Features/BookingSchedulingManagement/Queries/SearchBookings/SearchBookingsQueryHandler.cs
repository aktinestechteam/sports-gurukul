using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.SearchBookings;

public class SearchBookingsQueryHandler : IRequestHandler<SearchBookingsQuery, Result<(IReadOnlyList<BookingSummaryDto> Items, int TotalCount)>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<SearchBookingsQueryHandler> _logger;

    public SearchBookingsQueryHandler(
        IBookingRepository bookingRepository,
        ILogger<SearchBookingsQueryHandler> logger)
    {
        _bookingRepository = bookingRepository;
        _logger = logger;
    }

    public async Task<Result<(IReadOnlyList<BookingSummaryDto> Items, int TotalCount)>> Handle(
        SearchBookingsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Searching bookings - Page: {Page}, PageSize: {PageSize}", request.Page, request.PageSize);

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

        var summaries = bookings.Select(b => new BookingSummaryDto
        {
            Id = b.Id,
            BookingNumber = b.BookingNumber,
            BookingType = b.BookingType.ToString(),
            Status = b.Status.ToString(),
            Title = b.Title,
            AcademyId = b.AcademyId,
            AcademyName = b.Academy?.Name,
            FacilityName = b.Facility?.FacilityName,
            CoachName = b.Coach?.User?.FullName,
            AthleteName = b.Athlete?.User?.FullName,
            BookingDate = b.BookingDate,
            StartTime = b.StartTime,
            EndTime = b.EndTime,
            Duration = b.Duration,
            ApprovalStatus = b.ApprovalStatus.ToString(),
            CreatedAt = b.CreatedAt
        }).ToList();

        _logger.LogInformation("Found {Count} bookings (Total: {Total})", summaries.Count, totalCount);

        return Result<(IReadOnlyList<BookingSummaryDto>, int)>.Success((summaries, totalCount));
    }
}
