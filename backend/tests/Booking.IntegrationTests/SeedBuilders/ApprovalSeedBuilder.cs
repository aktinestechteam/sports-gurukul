using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace Booking.IntegrationTests.SeedBuilders;

public class ApprovalSeedBuilder
{
    private readonly BookingApproval _approval = new();

    public ApprovalSeedBuilder()
    {
        _approval.Id = Guid.NewGuid();
        _approval.BookingId = Guid.NewGuid();
        _approval.ApproverUserId = Guid.NewGuid();
        _approval.ApprovalStatus = BookingApprovalStatus.Pending;
        _approval.Comments = null;
        _approval.EscalationLevel = 0;
        _approval.CreatedAt = DateTime.UtcNow;
    }

    public ApprovalSeedBuilder WithId(Guid id)
    {
        _approval.Id = id;
        return this;
    }

    public ApprovalSeedBuilder WithBookingId(Guid bookingId)
    {
        _approval.BookingId = bookingId;
        return this;
    }

    public ApprovalSeedBuilder WithApproverUserId(Guid userId)
    {
        _approval.ApproverUserId = userId;
        return this;
    }

    public ApprovalSeedBuilder WithApprovalStatus(BookingApprovalStatus status)
    {
        _approval.ApprovalStatus = status;
        return this;
    }

    public ApprovalSeedBuilder WithComments(string? comments)
    {
        _approval.Comments = comments;
        return this;
    }

    public ApprovalSeedBuilder WithReviewedOn(DateTime? reviewedOn)
    {
        _approval.ReviewedOn = reviewedOn;
        return this;
    }

    public ApprovalSeedBuilder WithEscalationLevel(int level)
    {
        _approval.EscalationLevel = level;
        return this;
    }

    public BookingApproval Build() => _approval;
}
