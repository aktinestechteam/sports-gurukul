using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace Booking.IntegrationTests.SeedBuilders;

public class WaitlistSeedBuilder
{
    private readonly BookingWaitlist _waitlist = new();

    public WaitlistSeedBuilder()
    {
        _waitlist.Id = Guid.NewGuid();
        _waitlist.WaitlistUserId = Guid.NewGuid();
        _waitlist.Status = WaitlistStatus.Active;
        _waitlist.Priority = 1;
        _waitlist.CreatedAt = DateTime.UtcNow;
    }

    public WaitlistSeedBuilder WithId(Guid id)
    {
        _waitlist.Id = id;
        return this;
    }

    public WaitlistSeedBuilder WithBookingId(Guid bookingId)
    {
        _waitlist.BookingId = bookingId;
        return this;
    }

    public WaitlistSeedBuilder WithWaitlistUserId(Guid userId)
    {
        _waitlist.WaitlistUserId = userId;
        return this;
    }

    public WaitlistSeedBuilder WithStatus(WaitlistStatus status)
    {
        _waitlist.Status = status;
        return this;
    }

    public WaitlistSeedBuilder WithPriority(int priority)
    {
        _waitlist.Priority = priority;
        return this;
    }

    public WaitlistSeedBuilder WithNotes(string notes)
    {
        _waitlist.Notes = notes;
        return this;
    }

    public BookingWaitlist Build() => _waitlist;
}