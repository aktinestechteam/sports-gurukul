using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetFacilityBookings;

public class GetFacilityBookingsQueryHandler : IRequestHandler<GetFacilityBookingsQuery, Result<IReadOnlyList<BookingSummaryDto>>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<GetFacilityBookingsQueryHandler> _logger;

    public GetFacilityBookingsQueryHandler(
        IBookingRepository bookingRepository,
        ILogger<GetFacilityBookingsQueryHandler> logger)
    {
        _bookingRepository = bookingRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<BookingSummaryDto>>> Handle(
        GetFacilityBookingsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting bookings for facility {FacilityId} on {Date}", request.FacilityId, request.Date);

        var bookings = await _bookingRepository.GetByFacilityIdAsync(
            request.FacilityId, request.Date, cancellationToken);

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
