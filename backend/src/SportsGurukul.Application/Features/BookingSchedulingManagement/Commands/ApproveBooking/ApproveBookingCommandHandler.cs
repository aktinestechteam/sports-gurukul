using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CreateBooking;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.ApproveBooking;

public class ApproveBookingCommandHandler : IRequestHandler<ApproveBookingCommand, Result<BookingDto>>
{
    private readonly IBookingApprovalService _approvalService;
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<ApproveBookingCommandHandler> _logger;

    public ApproveBookingCommandHandler(
        IBookingApprovalService approvalService,
        IBookingRepository bookingRepository,
        ILogger<ApproveBookingCommandHandler> logger)
    {
        _approvalService = approvalService;
        _bookingRepository = bookingRepository;
        _logger = logger;
    }

    public async Task<Result<BookingDto>> Handle(ApproveBookingCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Approving booking {BookingId} by user {UserId}", request.BookingId, request.ApproverUserId);

        var processed = await _approvalService.ProcessApprovalAsync(
            request.BookingId, BookingApprovalStatus.Approved, request.ApproverUserId,
            request.Comments, cancellationToken);

        if (!processed)
            return Result<BookingDto>.Failure("Booking not found or cannot be approved.");

        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking is null)
            return Result<BookingDto>.Failure("Booking not found.");

        return Result<BookingDto>.Success(CreateBookingCommandHandler.MapToDto(booking));
    }
}
