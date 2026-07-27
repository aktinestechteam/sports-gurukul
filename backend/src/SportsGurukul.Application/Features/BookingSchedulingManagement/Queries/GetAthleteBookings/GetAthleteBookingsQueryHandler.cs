using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetAthleteBookings;

public class GetAthleteBookingsQueryHandler : IRequestHandler<GetAthleteBookingsQuery, Result<IReadOnlyList<BookingSummaryDto>>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<GetAthleteBookingsQueryHandler> _logger;

    public GetAthleteBookingsQueryHandler(
        IBookingRepository bookingRepository,
        ILogger<GetAthleteBookingsQueryHandler> logger)
    {
        _bookingRepository = bookingRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<BookingSummaryDto>>> Handle(
        GetAthleteBookingsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting bookings for athlete {AthleteId}", request.AthleteId);

        var bookings = await _bookingRepository.GetByAthleteIdAsync(request.AthleteId, cancellationToken);

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
