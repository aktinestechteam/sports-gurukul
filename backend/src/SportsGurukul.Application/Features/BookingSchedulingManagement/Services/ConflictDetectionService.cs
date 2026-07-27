using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Services;

public class ConflictDetectionService : IConflictDetectionService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IConflictRepository _conflictRepository;
    private readonly ILogger<ConflictDetectionService> _logger;

    public ConflictDetectionService(
        IBookingRepository bookingRepository,
        IConflictRepository conflictRepository,
        ILogger<ConflictDetectionService> logger)
    {
        _bookingRepository = bookingRepository;
        _conflictRepository = conflictRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<BookingConflict>> DetectConflictsAsync(
        Booking booking,
        CancellationToken cancellationToken = default)
    {
        var conflicts = new List<BookingConflict>();

        if (booking.FacilityId.HasValue)
        {
            var facilityBookings = await _bookingRepository
                .GetByFacilityIdAsync(booking.FacilityId.Value, booking.BookingDate, cancellationToken);

            foreach (var existing in facilityBookings.Where(b =>
                b.Id != booking.Id &&
                b.Status != BookingStatus.Cancelled &&
                b.Status != BookingStatus.Rejected &&
                b.StartTime < booking.EndTime &&
                b.EndTime > booking.StartTime))
            {
                conflicts.Add(new BookingConflict
                {
                    Id = Guid.NewGuid(),
                    BookingId = booking.Id,
                    ConflictingBookingId = existing.Id,
                    ConflictType = BookingConflictType.FacilityOverlap,
                    Description = $"Facility overlap with booking {existing.BookingNumber}",
                    IsResolved = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

        if (booking.CoachId.HasValue)
        {
            var coachBookings = await _bookingRepository
                .GetByCoachIdAsync(booking.CoachId.Value, booking.BookingDate, cancellationToken);

            foreach (var existing in coachBookings.Where(b =>
                b.Id != booking.Id &&
                b.Status != BookingStatus.Cancelled &&
                b.Status != BookingStatus.Rejected &&
                b.StartTime < booking.EndTime &&
                b.EndTime > booking.StartTime))
            {
                conflicts.Add(new BookingConflict
                {
                    Id = Guid.NewGuid(),
                    BookingId = booking.Id,
                    ConflictingBookingId = existing.Id,
                    ConflictType = BookingConflictType.CoachOverlap,
                    Description = $"Coach overlap with booking {existing.BookingNumber}",
                    IsResolved = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

        if (booking.AthleteId.HasValue)
        {
            var athleteBookings = await _bookingRepository
                .GetByAthleteIdAsync(booking.AthleteId.Value, cancellationToken);

            var overlappingAthleteBookings = athleteBookings
                .Where(b => b.Id != booking.Id &&
                           b.BookingDate.Date == booking.BookingDate.Date &&
                           b.Status != BookingStatus.Cancelled &&
                           b.Status != BookingStatus.Rejected &&
                           b.StartTime < booking.EndTime &&
                           b.EndTime > booking.StartTime)
                .ToList();

            foreach (var existing in overlappingAthleteBookings)
            {
                conflicts.Add(new BookingConflict
                {
                    Id = Guid.NewGuid(),
                    BookingId = booking.Id,
                    ConflictingBookingId = existing.Id,
                    ConflictType = BookingConflictType.AthleteOverlap,
                    Description = $"Athlete overlap with booking {existing.BookingNumber}",
                    IsResolved = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

        if (conflicts.Count > 0)
        {
            _logger.LogWarning(
                "Detected {Count} conflicts for booking {BookingNumber}",
                conflicts.Count, booking.BookingNumber);
        }

        return conflicts;
    }

    public async Task<IReadOnlyList<BookingConflict>> DetectConflictsForUpdateAsync(
        Booking booking,
        DateTime newDate,
        TimeSpan newStartTime,
        TimeSpan newEndTime,
        CancellationToken cancellationToken = default)
    {
        var updatedBooking = new Booking
        {
            Id = booking.Id,
            BookingDate = newDate,
            StartTime = newStartTime,
            EndTime = newEndTime,
            FacilityId = booking.FacilityId,
            CoachId = booking.CoachId,
            AthleteId = booking.AthleteId,
            BookingNumber = booking.BookingNumber
        };

        return await DetectConflictsAsync(updatedBooking, cancellationToken);
    }

    public async Task<bool> HasConflictsAsync(
        Guid bookingId,
        CancellationToken cancellationToken = default)
    {
        var unresolved = await _conflictRepository
            .GetUnresolvedByBookingIdAsync(bookingId, cancellationToken);
        return unresolved.Count > 0;
    }
}
