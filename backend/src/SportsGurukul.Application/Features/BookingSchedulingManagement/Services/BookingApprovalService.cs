using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Services;

public class BookingApprovalService : IBookingApprovalService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BookingApprovalService> _logger;

    public BookingApprovalService(
        IBookingRepository bookingRepository,
        IUnitOfWork unitOfWork,
        ILogger<BookingApprovalService> logger)
    {
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<BookingApproval> CreateApprovalRequestAsync(
        Guid bookingId,
        BookingApprovalStatus status,
        string? comments = null,
        CancellationToken cancellationToken = default)
    {
        var approval = new BookingApproval
        {
            Id = Guid.NewGuid(),
            BookingId = bookingId,
            ApprovalStatus = status,
            Comments = comments,
            EscalationLevel = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _logger.LogInformation(
            "Created approval request for booking {BookingId} with status {Status}",
            bookingId, status);

        return approval;
    }

    public async Task<bool> ProcessApprovalAsync(
        Guid bookingId,
        BookingApprovalStatus status,
        Guid approverUserId,
        string? comments = null,
        CancellationToken cancellationToken = default)
    {
        var booking = await _bookingRepository.GetWithDetailsAsync(bookingId, cancellationToken);
        if (booking is null)
        {
            _logger.LogWarning("Booking {BookingId} not found for approval processing", bookingId);
            return false;
        }

        var lastApproval = booking.Approvals
            .OrderByDescending(a => a.CreatedAt)
            .FirstOrDefault();

        if (lastApproval is not null)
        {
            lastApproval.ApprovalStatus = status;
            lastApproval.ApproverUserId = approverUserId;
            lastApproval.ReviewedOn = DateTime.UtcNow;
            lastApproval.Comments = comments;
            lastApproval.UpdatedAt = DateTime.UtcNow;
        }

        booking.ApprovalStatus = status;
        booking.UpdatedAt = DateTime.UtcNow;

        _logger.LogInformation(
            "Processed approval for booking {BookingId}: {Status}",
            bookingId, status);

        return true;
    }
}
