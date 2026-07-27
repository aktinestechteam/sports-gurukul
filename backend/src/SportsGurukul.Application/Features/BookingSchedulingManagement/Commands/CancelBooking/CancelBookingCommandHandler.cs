using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CreateBooking;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CancelBooking;

public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, Result<BookingDto>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IWaitlistService _waitlistService;
    private readonly IWaitlistRepository _waitlistRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CancelBookingCommandHandler> _logger;

    public CancelBookingCommandHandler(
        IBookingRepository bookingRepository,
        IWaitlistService waitlistService,
        IWaitlistRepository waitlistRepository,
        IUnitOfWork unitOfWork,
        ILogger<CancelBookingCommandHandler> logger)
    {
        _bookingRepository = bookingRepository;
        _waitlistService = waitlistService;
        _waitlistRepository = waitlistRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<BookingDto>> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling booking {BookingId}", request.BookingId);

        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking is null)
            return Result<BookingDto>.Failure("Booking not found.");

        if (booking.Status == BookingStatus.Cancelled)
            return Result<BookingDto>.Failure("Booking is already cancelled.");

        if (booking.Status == BookingStatus.Completed)
            return Result<BookingDto>.Failure("Cannot cancel a completed booking.");

        var previousStatus = booking.Status.ToString();
        booking.Status = BookingStatus.Cancelled;
        booking.UpdatedAt = DateTime.UtcNow;

        var cancellation = new BookingCancellation
        {
            Id = Guid.NewGuid(),
            BookingId = booking.Id,
            CancelledByUserId = booking.BookingCreatorId ?? Guid.Empty,
            CancelledOn = DateTime.UtcNow,
            Reason = request.Reason,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _bookingRepository.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Booking cancelled: {BookingNumber} (Status: {PreviousStatus} -> Cancelled)",
            booking.BookingNumber, previousStatus);

        return Result<BookingDto>.Success(CreateBookingCommandHandler.MapToDto(booking));
    }
}
