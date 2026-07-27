using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Services;

public interface IBookingApprovalService
{
    Task<BookingApproval> CreateApprovalRequestAsync(
        Guid bookingId,
        BookingApprovalStatus status,
        string? comments = null,
        CancellationToken cancellationToken = default);
    Task<bool> ProcessApprovalAsync(
        Guid bookingId,
        BookingApprovalStatus status,
        Guid approverUserId,
        string? comments = null,
        CancellationToken cancellationToken = default);
}
