namespace SportsGurukul.Platform.Competition.Interfaces.Scheduling;

public interface ISchedulingEngine
{
    Task<IReadOnlyList<ScheduledSlot>> FindAvailableSlotsAsync(
        Guid venueId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default);
}

public interface IAvailabilityService
{
    Task<bool> IsParticipantAvailableAsync(
        Guid participantId,
        DateTime date,
        TimeSpan time,
        CancellationToken cancellationToken = default);

    Task<bool> IsVenueAvailableAsync(
        Guid venueId,
        DateTime date,
        TimeSpan time,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Guid>> GetAvailableVenuesAsync(
        DateTime date,
        TimeSpan time,
        CancellationToken cancellationToken = default);
}

public interface IConflictDetectionService
{
    Task<bool> HasConflictAsync(
        Guid participantId,
        DateTime date,
        TimeSpan time,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ConflictInfo>> DetectConflictsAsync(
        IReadOnlyList<Guid> participantIds,
        DateTime date,
        TimeSpan time,
        CancellationToken cancellationToken = default);
}

public class ScheduledSlot
{
    public DateTime Date { get; set; }
    public TimeSpan Time { get; set; }
    public Guid VenueId { get; set; }
    public bool IsAvailable { get; set; }
}

public class ConflictInfo
{
    public Guid ParticipantId { get; set; }
    public string? ParticipantName { get; set; }
    public string? ConflictType { get; set; }
    public DateTime ConflictDate { get; set; }
}
