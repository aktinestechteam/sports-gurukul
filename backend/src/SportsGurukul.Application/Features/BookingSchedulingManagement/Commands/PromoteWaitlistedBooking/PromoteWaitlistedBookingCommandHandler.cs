using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CreateBooking;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.PromoteWaitlistedBooking;

public class PromoteWaitlistedBookingCommandHandler : IRequestHandler<PromoteWaitlistedBookingCommand, Result<BookingDto>>
{
    private readonly IWaitlistRepository _waitlistRepository;
    private readonly IWaitlistService _waitlistService;
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PromoteWaitlistedBookingCommandHandler> _logger;

    public PromoteWaitlistedBookingCommandHandler(
        IWaitlistRepository waitlistRepository,
        IWaitlistService waitlistService,
        IBookingRepository bookingRepository,
        IUnitOfWork unitOfWork,
        ILogger<PromoteWaitlistedBookingCommandHandler> logger)
    {
        _waitlistRepository = waitlistRepository;
        _waitlistService = waitlistService;
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<BookingDto>> Handle(PromoteWaitlistedBookingCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Promoting waitlist entry {WaitlistEntryId}", request.WaitlistEntryId);

        var entry = await _waitlistRepository.GetByIdAsync(request.WaitlistEntryId, cancellationToken);
        if (entry is null)
            return Result<BookingDto>.Failure("Waitlist entry not found.");

        if (entry.Status != WaitlistStatus.Active)
            return Result<BookingDto>.Failure("Only active waitlist entries can be promoted.");

        var promoted = await _waitlistService.PromoteWaitlistedBookingAsync(entry, cancellationToken);
        if (!promoted)
            return Result<BookingDto>.Failure("Failed to promote waitlist entry.");

        _waitlistRepository.Update(entry);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var booking = await _bookingRepository.GetByIdAsync(entry.BookingId, cancellationToken);
        if (booking is null)
            return Result<BookingDto>.Failure("Booking not found.");

        _logger.LogInformation("Waitlist entry promoted for booking {BookingNumber}", booking.BookingNumber);

        return Result<BookingDto>.Success(CreateBookingCommandHandler.MapToDto(booking));
    }
}
