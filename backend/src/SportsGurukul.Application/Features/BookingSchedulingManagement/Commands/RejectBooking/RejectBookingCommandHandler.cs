using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CreateBooking;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.RejectBooking;

public class RejectBookingCommandHandler : IRequestHandler<RejectBookingCommand, Result<BookingDto>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RejectBookingCommandHandler> _logger;

    public RejectBookingCommandHandler(
        IBookingRepository bookingRepository,
        IUnitOfWork unitOfWork,
        ILogger<RejectBookingCommandHandler> logger)
    {
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<BookingDto>> Handle(RejectBookingCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Rejecting booking {BookingId}", request.BookingId);

        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking is null)
            return Result<BookingDto>.Failure("Booking not found.");

        if (booking.Status != BookingStatus.Pending)
            return Result<BookingDto>.Failure("Only pending bookings can be rejected.");

        booking.Status = BookingStatus.Rejected;
        booking.ApprovalStatus = BookingApprovalStatus.Rejected;
        booking.UpdatedAt = DateTime.UtcNow;

        _bookingRepository.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Booking rejected: {BookingNumber}", booking.BookingNumber);

        return Result<BookingDto>.Success(CreateBookingCommandHandler.MapToDto(booking));
    }
}
