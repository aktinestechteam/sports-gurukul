using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Services;

public class SchedulingEngine : ISchedulingEngine
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IBookingScheduleRepository _bookingScheduleRepository;
    private readonly IConflictDetectionService _conflictDetectionService;
    private readonly ILogger<SchedulingEngine> _logger;

    public SchedulingEngine(
        IBookingRepository bookingRepository,
        IBookingScheduleRepository bookingScheduleRepository,
        IConflictDetectionService conflictDetectionService,
        ILogger<SchedulingEngine> logger)
    {
        _bookingRepository = bookingRepository;
        _bookingScheduleRepository = bookingScheduleRepository;
        _conflictDetectionService = conflictDetectionService;
        _logger = logger;
    }

    public async Task<string> GenerateBookingNumberAsync(CancellationToken cancellationToken = default)
    {
        string bookingNumber;
        do
        {
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var randomPart = Guid.NewGuid().ToString("N")[..4].ToUpperInvariant();
            bookingNumber = $"BK-{datePart}-{randomPart}";
        }
        while (await _bookingRepository.IsBookingNumberUniqueAsync(bookingNumber, cancellationToken) == false);

        return bookingNumber;
    }

    public async Task<IReadOnlyList<BookingSchedule>> GenerateScheduleInstancesAsync(
        Booking booking,
        DateTime startDate,
        TimeSpan startTime,
        TimeSpan endTime,
        int? occurrenceCount,
        DateTime? endDate,
        CancellationToken cancellationToken = default)
    {
        var schedules = new List<BookingSchedule>();
        var current = startDate;
        var count = 0;
        var maxOccurrences = occurrenceCount ?? 365;
        var finalDate = endDate ?? startDate.AddYears(1);

        while (count < maxOccurrences && current.Date <= finalDate.Date)
        {
            var schedule = new BookingSchedule
            {
                Id = Guid.NewGuid(),
                BookingId = booking.Id,
                ScheduledDate = current,
                StartTime = startTime,
                EndTime = endTime,
                IsCancelled = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            schedules.Add(schedule);
            count++;
            current = current.AddDays(1);
        }

        _logger.LogInformation(
            "Generated {Count} schedule instances for booking {BookingNumber}",
            schedules.Count, booking.BookingNumber);

        return schedules;
    }

    public async Task<bool> IsSlotAvailableAsync(
        Guid academyId,
        Guid? facilityId,
        Guid? coachId,
        DateTime date,
        TimeSpan startTime,
        TimeSpan endTime,
        Guid? excludeBookingId = null,
        CancellationToken cancellationToken = default)
    {
        var facilityBookings = facilityId.HasValue
            ? await _bookingRepository.GetByFacilityIdAsync(facilityId.Value, date, cancellationToken)
            : new List<Booking>();

        var coachBookings = coachId.HasValue
            ? await _bookingRepository.GetByCoachIdAsync(coachId.Value, date, cancellationToken)
            : new List<Booking>();

        var allRelevantBookings = facilityBookings
            .Concat(coachBookings)
            .Where(b => b.Status != BookingStatus.Cancelled
                     && b.Status != BookingStatus.Rejected
                     && (!excludeBookingId.HasValue || b.Id != excludeBookingId.Value))
            .ToList();

        return !allRelevantBookings.Any(b =>
            b.BookingDate.Date == date.Date &&
            b.StartTime < endTime &&
            b.EndTime > startTime);
    }
}
