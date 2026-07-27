using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetBookingStatistics;

public class GetBookingStatisticsQueryHandler : IRequestHandler<GetBookingStatisticsQuery, Result<BookingStatisticsDto>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<GetBookingStatisticsQueryHandler> _logger;

    public GetBookingStatisticsQueryHandler(
        IBookingRepository bookingRepository,
        ILogger<GetBookingStatisticsQueryHandler> logger)
    {
        _bookingRepository = bookingRepository;
        _logger = logger;
    }

    public async Task<Result<BookingStatisticsDto>> Handle(
        GetBookingStatisticsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting booking statistics for academy {AcademyId}", request.AcademyId);

        var startDate = request.StartDate ?? DateTime.UtcNow.AddMonths(-1);
        var endDate = request.EndDate ?? DateTime.UtcNow;

        var allBookings = await _bookingRepository.GetByDateRangeAsync(
            request.AcademyId, startDate, endDate, cancellationToken);

        var totalBookings = allBookings.Count;
        var dailyBookings = allBookings.Count(b => b.BookingDate.Date == DateTime.UtcNow.Date);
        var monthlyBookings = allBookings.Count(b => b.BookingDate.Month == DateTime.UtcNow.Month
                                                    && b.BookingDate.Year == DateTime.UtcNow.Year);

        var cancelledBookings = allBookings.Count(b => b.Status == BookingStatus.Cancelled);
        var cancellationRate = totalBookings > 0
            ? (decimal)cancelledBookings / totalBookings * 100
            : 0;

        var confirmedBookings = allBookings.Count(b => b.Status == BookingStatus.Confirmed);
        var completedBookings = allBookings.Count(b => b.Status == BookingStatus.Completed);
        var facilityUtilization = totalBookings > 0
            ? (decimal)(confirmedBookings + completedBookings) / totalBookings * 100
            : 0;

        var coachBookings = allBookings.Where(b => b.CoachId.HasValue).ToList();
        var coachUtilization = coachBookings.Count > 0
            ? (decimal)coachBookings.Count(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed) / coachBookings.Count * 100
            : 0;

        var peakHours = allBookings
            .GroupBy(b => b.StartTime.Hours)
            .Select(g => new PeakHourDto { Hour = g.Key, BookingCount = g.Count() })
            .OrderByDescending(p => p.BookingCount)
            .Take(5)
            .ToList();

        var facilityOccupancy = allBookings
            .Where(b => b.FacilityId.HasValue)
            .GroupBy(b => new { b.FacilityId, b.Facility?.FacilityName })
            .Select(g => new FacilityOccupancyDto
            {
                FacilityId = g.Key.FacilityId ?? Guid.Empty,
                FacilityName = g.Key.FacilityName,
                OccupancyPercent = totalBookings > 0 ? (decimal)g.Count() / totalBookings * 100 : 0
            })
            .ToList();

        return Result<BookingStatisticsDto>.Success(new BookingStatisticsDto
        {
            TotalBookings = totalBookings,
            DailyBookings = dailyBookings,
            MonthlyBookings = monthlyBookings,
            FacilityUtilizationPercent = facilityUtilization,
            CoachUtilizationPercent = coachUtilization,
            CancellationRate = cancellationRate,
            WaitlistConversionRate = 0,
            PeakBookingHours = peakHours,
            FacilityOccupancy = facilityOccupancy
        });
    }
}
