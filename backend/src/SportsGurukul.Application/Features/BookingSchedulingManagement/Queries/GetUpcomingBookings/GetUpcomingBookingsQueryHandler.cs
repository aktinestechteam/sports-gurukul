using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetUpcomingBookings;

public class GetUpcomingBookingsQueryHandler : IRequestHandler<GetUpcomingBookingsQuery, Result<IReadOnlyList<BookingSummaryDto>>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<GetUpcomingBookingsQueryHandler> _logger;

    public GetUpcomingBookingsQueryHandler(
        IBookingRepository bookingRepository,
        ILogger<GetUpcomingBookingsQueryHandler> logger)
    {
        _bookingRepository = bookingRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<BookingSummaryDto>>> Handle(
        GetUpcomingBookingsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting upcoming bookings for academy {AcademyId} for {Days} days",
            request.AcademyId, request.DaysAhead);

        var startDate = DateTime.UtcNow.Date;
        var endDate = startDate.AddDays(request.DaysAhead);

        var bookings = await _bookingRepository.GetByDateRangeAsync(
            request.AcademyId, startDate, endDate, cancellationToken);

        var upcomingBookings = bookings
            .Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending)
            .OrderBy(b => b.BookingDate)
            .ThenBy(b => b.StartTime)
            .ToList();

        var summaries = upcomingBookings.Select(b => new BookingSummaryDto
        {
            Id = b.Id,
            BookingNumber = b.BookingNumber,
            BookingType = b.BookingType.ToString(),
            Status = b.Status.ToString(),
            Title = b.Title,
            AcademyId = b.AcademyId,
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

        return Result<IReadOnlyList<BookingSummaryDto>>.Success(summaries);
    }
}
