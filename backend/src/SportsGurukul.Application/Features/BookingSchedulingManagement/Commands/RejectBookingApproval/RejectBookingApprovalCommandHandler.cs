using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CreateBooking;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.RejectBookingApproval;

public class RejectBookingApprovalCommandHandler : IRequestHandler<RejectBookingApprovalCommand, Result<BookingDto>>
{
    private readonly IBookingApprovalService _approvalService;
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<RejectBookingApprovalCommandHandler> _logger;

    public RejectBookingApprovalCommandHandler(
        IBookingApprovalService approvalService,
        IBookingRepository bookingRepository,
        ILogger<RejectBookingApprovalCommandHandler> logger)
    {
        _approvalService = approvalService;
        _bookingRepository = bookingRepository;
        _logger = logger;
    }

    public async Task<Result<BookingDto>> Handle(RejectBookingApprovalCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Rejecting booking approval {BookingId} by user {UserId}", request.BookingId, request.ApproverUserId);

        var processed = await _approvalService.ProcessApprovalAsync(
            request.BookingId, BookingApprovalStatus.Rejected, request.ApproverUserId,
            request.Comments, cancellationToken);

        if (!processed)
            return Result<BookingDto>.Failure("Booking not found or cannot be rejected.");

        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking is null)
            return Result<BookingDto>.Failure("Booking not found.");

        booking.Status = BookingStatus.Rejected;
        booking.UpdatedAt = DateTime.UtcNow;

        return Result<BookingDto>.Success(CreateBookingCommandHandler.MapToDto(booking));
    }
}
