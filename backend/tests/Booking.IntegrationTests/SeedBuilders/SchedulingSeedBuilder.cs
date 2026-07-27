using SportsGurukul.Domain.Entities;

namespace Booking.IntegrationTests.SeedBuilders;

public class SchedulingSeedBuilder
{
    private readonly BookingSchedule _schedule = new();

    public SchedulingSeedBuilder()
    {
        _schedule.Id = Guid.NewGuid();
        _schedule.ScheduledDate = DateTime.UtcNow.Date.AddDays(1);
        _schedule.StartTime = new TimeSpan(9, 0, 0);
        _schedule.EndTime = new TimeSpan(10, 30, 0);
        _schedule.IsCancelled = false;
        _schedule.CreatedAt = DateTime.UtcNow;
    }

    public SchedulingSeedBuilder WithId(Guid id)
    {
        _schedule.Id = id;
        return this;
    }

    public SchedulingSeedBuilder WithBookingId(Guid bookingId)
    {
        _schedule.BookingId = bookingId;
        return this;
    }

    public SchedulingSeedBuilder WithScheduledDate(DateTime date)
    {
        _schedule.ScheduledDate = date;
        return this;
    }

    public SchedulingSeedBuilder WithStartTime(TimeSpan start)
    {
        _schedule.StartTime = start;
        return this;
    }

    public SchedulingSeedBuilder WithEndTime(TimeSpan end)
    {
        _schedule.EndTime = end;
        return this;
    }

    public SchedulingSeedBuilder WithIsCancelled(bool isCancelled)
    {
        _schedule.IsCancelled = isCancelled;
        return this;
    }

    public SchedulingSeedBuilder WithCancellationReason(string reason)
    {
        _schedule.CancellationReason = reason;
        return this;
    }

    public SchedulingSeedBuilder WithNotes(string notes)
    {
        _schedule.Notes = notes;
        return this;
    }

    public BookingSchedule Build() => _schedule;
}
