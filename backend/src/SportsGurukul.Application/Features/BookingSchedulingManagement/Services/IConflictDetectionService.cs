using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Services;

public interface IConflictDetectionService
{
    Task<IReadOnlyList<BookingConflict>> DetectConflictsAsync(
        Booking booking,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookingConflict>> DetectConflictsForUpdateAsync(
        Booking booking,
        DateTime newDate,
        TimeSpan newStartTime,
        TimeSpan newEndTime,
        CancellationToken cancellationToken = default);
    Task<bool> HasConflictsAsync(
        Guid bookingId,
        CancellationToken cancellationToken = default);
}

public record ConflictCheckResult
{
    public bool HasConflict { get; init; }
    public BookingConflictType ConflictType { get; init; }
    public string? Description { get; init; }
    public Guid? ConflictingBookingId { get; init; }
}
