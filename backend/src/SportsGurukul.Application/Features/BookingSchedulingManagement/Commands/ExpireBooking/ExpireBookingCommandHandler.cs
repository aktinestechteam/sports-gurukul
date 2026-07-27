using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CreateBooking;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.ExpireBooking;

public class ExpireBookingCommandHandler : IRequestHandler<ExpireBookingCommand, Result<BookingDto>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ExpireBookingCommandHandler> _logger;

    public ExpireBookingCommandHandler(
        IBookingRepository bookingRepository,
        IUnitOfWork unitOfWork,
        ILogger<ExpireBookingCommandHandler> logger)
    {
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<BookingDto>> Handle(ExpireBookingCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Expiring booking {BookingId}", request.BookingId);

        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking is null)
            return Result<BookingDto>.Failure("Booking not found.");

        if (booking.Status != BookingStatus.Pending && booking.Status != BookingStatus.Draft)
            return Result<BookingDto>.Failure("Only draft or pending bookings can be expired.");

        booking.Status = BookingStatus.Expired;
        booking.UpdatedAt = DateTime.UtcNow;

        _bookingRepository.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Booking expired: {BookingNumber}", booking.BookingNumber);

        return Result<BookingDto>.Success(CreateBookingCommandHandler.MapToDto(booking));
    }
}
