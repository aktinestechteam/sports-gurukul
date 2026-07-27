using SportsGurukul.Domain.Enums;

namespace Booking.IntegrationTests.SeedBuilders;

public class BookingSeedBuilder
{
    private readonly SportsGurukul.Domain.Entities.Booking _booking = new();

    public BookingSeedBuilder()
    {
        _booking.Id = Guid.NewGuid();
        _booking.BookingNumber = "BK-TEST";
        _booking.BookingType = BookingType.FacilityReservation;
        _booking.Status = BookingStatus.Pending;
        _booking.Title = "Test Booking";
        _booking.Description = "Test description";
        _booking.BookingDate = DateTime.UtcNow.Date.AddDays(1);
        _booking.StartTime = new TimeSpan(9, 0, 0);
        _booking.EndTime = new TimeSpan(10, 30, 0);
        _booking.Duration = 90;
        _booking.ApprovalStatus = BookingApprovalStatus.Pending;
    }

    public BookingSeedBuilder WithId(Guid id)
    {
        _booking.Id = id;
        return this;
    }

    public BookingSeedBuilder WithBookingNumber(string number)
    {
        _booking.BookingNumber = number;
        return this;
    }

    public BookingSeedBuilder WithType(BookingType type)
    {
        _booking.BookingType = type;
        return this;
    }

    public BookingSeedBuilder WithStatus(BookingStatus status)
    {
        _booking.Status = status;
        return this;
    }

    public BookingSeedBuilder WithTitle(string title)
    {
        _booking.Title = title;
        return this;
    }

    public BookingSeedBuilder WithDescription(string description)
    {
        _booking.Description = description;
        return this;
    }

    public BookingSeedBuilder WithAcademyId(Guid academyId)
    {
        _booking.AcademyId = academyId;
        return this;
    }

    public BookingSeedBuilder WithBranchId(Guid branchId)
    {
        _booking.BranchId = branchId;
        return this;
    }

    public BookingSeedBuilder WithFacilityId(Guid facilityId)
    {
        _booking.FacilityId = facilityId;
        return this;
    }

    public BookingSeedBuilder WithCoachId(Guid coachId)
    {
        _booking.CoachId = coachId;
        return this;
    }

    public BookingSeedBuilder WithAthleteId(Guid athleteId)
    {
        _booking.AthleteId = athleteId;
        return this;
    }

    public BookingSeedBuilder WithBookingDate(DateTime date)
    {
        _booking.BookingDate = date;
        return this;
    }

    public BookingSeedBuilder WithStartTime(TimeSpan start)
    {
        _booking.StartTime = start;
        return this;
    }

    public BookingSeedBuilder WithEndTime(TimeSpan end)
    {
        _booking.EndTime = end;
        return this;
    }

    public BookingSeedBuilder WithDuration(int duration)
    {
        _booking.Duration = duration;
        return this;
    }

    public BookingSeedBuilder WithApprovalStatus(BookingApprovalStatus status)
    {
        _booking.ApprovalStatus = status;
        return this;
    }

    public BookingSeedBuilder WithBookingCreatorId(Guid creatorId)
    {
        _booking.BookingCreatorId = creatorId;
        return this;
    }

    public SportsGurukul.Domain.Entities.Booking Build() => _booking;
}