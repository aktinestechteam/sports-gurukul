using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetCoachBookings;

public class GetCoachBookingsQueryHandler : IRequestHandler<GetCoachBookingsQuery, Result<IReadOnlyList<BookingSummaryDto>>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<GetCoachBookingsQueryHandler> _logger;

    public GetCoachBookingsQueryHandler(
        IBookingRepository bookingRepository,
        ILogger<GetCoachBookingsQueryHandler> logger)
    {
        _bookingRepository = bookingRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<BookingSummaryDto>>> Handle(
        GetCoachBookingsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting bookings for coach {CoachId} on {Date}", request.CoachId, request.Date);

        var bookings = await _bookingRepository.GetByCoachIdAsync(
            request.CoachId, request.Date, cancellationToken);

        var summaries = bookings.Select(b => new BookingSummaryDto
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
