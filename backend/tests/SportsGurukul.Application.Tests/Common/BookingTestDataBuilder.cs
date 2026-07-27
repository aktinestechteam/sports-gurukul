using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Common;

public static class BookingTestDataBuilder
{
    public static Booking CreateBooking(
        Guid? id = null,
        Guid? facilityId = null,
        Guid? coachId = null,
        Guid? athleteId = null,
        BookingStatus status = BookingStatus.Pending,
        DateTime? bookingDate = null,
        TimeSpan? startTime = null,
        TimeSpan? endTime = null,
        string? bookingNumber = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        BookingNumber = bookingNumber ?? $"BK-{DateTime.UtcNow:yyyyMMdd}-TEST",
        BookingType = BookingType.TrainingSession,
        Status = status,
        Title = "Test Booking",
        Description = "Test description",
        AcademyId = Guid.NewGuid(),
        FacilityId = facilityId,
        CoachId = coachId,
        AthleteId = athleteId,
        BookingDate = bookingDate ?? DateTime.UtcNow.Date.AddDays(1),
        StartTime = startTime ?? TimeSpan.FromHours(9),
        EndTime = endTime ?? TimeSpan.FromHours(10),
        Duration = (int)((endTime ?? TimeSpan.FromHours(10)) - (startTime ?? TimeSpan.FromHours(9))).TotalMinutes,
        ApprovalStatus = BookingApprovalStatus.Pending,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        Schedules = new List<BookingSchedule>(),
        Items = new List<BookingItem>(),
        Participants = new List<BookingParticipant>(),
        Recurrences = new List<BookingRecurrence>(),
        WaitlistEntries = new List<BookingWaitlist>(),
        Cancellations = new List<BookingCancellation>(),
        Reschedules = new List<BookingReschedule>(),
        Reminders = new List<BookingReminder>(),
        Approvals = new List<BookingApproval>(),
        Conflicts = new List<BookingConflict>(),
        History = new List<BookingHistory>(),
        Attachments = new List<BookingAttachment>()
    };

    public static Booking CreateConfirmedBooking(
        Guid? facilityId = null,
        Guid? coachId = null,
        Guid? athleteId = null,
        DateTime? bookingDate = null,
        TimeSpan? startTime = null,
        TimeSpan? endTime = null) => CreateBooking(
            facilityId: facilityId,
            coachId: coachId,
            athleteId: athleteId,
            status: BookingStatus.Confirmed,
            bookingDate: bookingDate,
            startTime: startTime,
            endTime: endTime);

    public static Booking CreateCrossMidnightBooking(
        Guid? facilityId = null,
        Guid? coachId = null) => CreateBooking(
            facilityId: facilityId,
            coachId: coachId,
            status: BookingStatus.Confirmed,
            bookingDate: DateTime.UtcNow.Date.AddDays(1),
            startTime: TimeSpan.FromHours(22),
            endTime: new TimeSpan(1, 0, 0));

    public static Booking CreateMaxDurationBooking() => CreateBooking(
        facilityId: Guid.NewGuid(),
        status: BookingStatus.Confirmed,
        bookingDate: DateTime.UtcNow.Date.AddDays(1),
        startTime: TimeSpan.FromHours(6),
        endTime: new TimeSpan(22, 0, 0));

    public static Booking CreateOverlappingBooking(
        Booking existing,
        BookingConflictType conflictType = BookingConflictType.FacilityOverlap) => new()
    {
        Id = Guid.NewGuid(),
        BookingNumber = $"BK-{DateTime.UtcNow:yyyyMMdd}-OVLP",
        BookingType = BookingType.TrainingSession,
        Status = BookingStatus.Confirmed,
        Title = "Overlapping Booking",
        AcademyId = existing.AcademyId,
        FacilityId = conflictType == BookingConflictType.FacilityOverlap ? existing.FacilityId : Guid.NewGuid(),
        CoachId = conflictType == BookingConflictType.CoachOverlap ? existing.CoachId : Guid.NewGuid(),
        AthleteId = conflictType == BookingConflictType.AthleteOverlap ? existing.AthleteId : Guid.NewGuid(),
        BookingDate = existing.BookingDate,
        StartTime = existing.StartTime.Add(TimeSpan.FromMinutes(30)),
        EndTime = existing.EndTime.Add(TimeSpan.FromMinutes(30)),
        Duration = 60,
        ApprovalStatus = BookingApprovalStatus.Pending,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    public static Facility CreateFacility(
        Guid? id = null,
        FacilityStatus status = FacilityStatus.Active,
        bool isDeleted = false) => new()
    {
        Id = id ?? Guid.NewGuid(),
        AcademyId = Guid.NewGuid(),
        FacilityCode = $"FAC-{DateTime.UtcNow:yyyyMMdd}-TEST",
        FacilityName = "Test Facility",
        FacilityType = FacilityType.OutdoorGround,
        Capacity = 100,
        Status = status,
        IsDeleted = isDeleted,
        Areas = new List<FacilityArea>(),
        Courts = new List<FacilityCourt>(),
        Equipment = new List<FacilityEquipment>(),
        Schedules = new List<FacilitySchedule>(),
        PricingTiers = new List<FacilityPricing>(),
        Images = new List<FacilityImage>(),
        Amenities = new List<FacilityAmenity>(),
        Reviews = new List<FacilityReview>()
    };

    public static BookingSchedule CreateBookingSchedule(
        Guid? bookingId = null,
        DateTime? scheduledDate = null,
        TimeSpan? startTime = null,
        TimeSpan? endTime = null,
        bool isCancelled = false) => new()
    {
        Id = Guid.NewGuid(),
        BookingId = bookingId ?? Guid.NewGuid(),
        ScheduledDate = scheduledDate ?? DateTime.UtcNow.Date.AddDays(1),
        StartTime = startTime ?? TimeSpan.FromHours(9),
        EndTime = endTime ?? TimeSpan.FromHours(10),
        IsCancelled = isCancelled,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    public static BookingConflict CreateBookingConflict(
        Guid? bookingId = null,
        Guid? conflictingBookingId = null,
        BookingConflictType conflictType = BookingConflictType.FacilityOverlap,
        bool isResolved = false) => new()
    {
        Id = Guid.NewGuid(),
        BookingId = bookingId ?? Guid.NewGuid(),
        ConflictingBookingId = conflictingBookingId ?? Guid.NewGuid(),
        ConflictType = conflictType,
        Description = "Test conflict",
        IsResolved = isResolved,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    public static BookingWaitlist CreateBookingWaitlist(
        Guid? bookingId = null,
        int priority = 1,
        WaitlistStatus status = WaitlistStatus.Active) => new()
    {
        Id = Guid.NewGuid(),
        BookingId = bookingId ?? Guid.NewGuid(),
        WaitlistUserId = Guid.NewGuid(),
        Priority = priority,
        RequestedOn = DateTime.UtcNow.AddHours(-priority),
        PromotionOrder = 0,
        Status = status,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    public static BookingApproval CreateBookingApproval(
        Guid? bookingId = null,
        BookingApprovalStatus status = BookingApprovalStatus.Pending,
        Guid? approverUserId = null,
        string? comments = null) => new()
    {
        Id = Guid.NewGuid(),
        BookingId = bookingId ?? Guid.NewGuid(),
        ApprovalStatus = status,
        ApproverUserId = approverUserId,
        Comments = comments,
        EscalationLevel = 0,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
}
