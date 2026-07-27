using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.JoinWaitlist;

public class JoinWaitlistCommandHandler : IRequestHandler<JoinWaitlistCommand, Result<WaitlistDto>>
{
    private readonly IWaitlistRepository _waitlistRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IWaitlistService _waitlistService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<JoinWaitlistCommandHandler> _logger;

    public JoinWaitlistCommandHandler(
        IWaitlistRepository waitlistRepository,
        IBookingRepository bookingRepository,
        IWaitlistService waitlistService,
        IUnitOfWork unitOfWork,
        ILogger<JoinWaitlistCommandHandler> logger)
    {
        _waitlistRepository = waitlistRepository;
        _bookingRepository = bookingRepository;
        _waitlistService = waitlistService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<WaitlistDto>> Handle(JoinWaitlistCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("User {UserId} joining waitlist for booking {BookingId}", request.WaitlistUserId, request.BookingId);

        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking is null)
            return Result<WaitlistDto>.Failure("Booking not found.");

        if (booking.Status == BookingStatus.Cancelled)
            return Result<WaitlistDto>.Failure("Cannot join waitlist for a cancelled booking.");

        var existing = await _waitlistRepository.GetByBookingAndUserAsync(
            request.BookingId, request.WaitlistUserId, cancellationToken);
        if (existing is not null)
            return Result<WaitlistDto>.Failure("You are already on the waitlist for this booking.");

        var priority = await _waitlistService.GetNextPriorityAsync(request.BookingId, cancellationToken);

        var waitlistEntry = new BookingWaitlist
        {
            Id = Guid.NewGuid(),
            BookingId = request.BookingId,
            WaitlistUserId = request.WaitlistUserId,
            Priority = priority,
            RequestedOn = DateTime.UtcNow,
            PromotionOrder = 0,
            Status = WaitlistStatus.Active,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _waitlistRepository.AddAsync(waitlistEntry, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserId} added to waitlist for booking {BookingNumber} with priority {Priority}",
            request.WaitlistUserId, booking.BookingNumber, priority);

        return Result<WaitlistDto>.Success(new WaitlistDto
        {
            Id = waitlistEntry.Id,
            BookingId = waitlistEntry.BookingId,
            WaitlistUserId = waitlistEntry.WaitlistUserId,
            Priority = waitlistEntry.Priority,
            RequestedOn = waitlistEntry.RequestedOn,
            PromotionOrder = waitlistEntry.PromotionOrder,
            Status = waitlistEntry.Status.ToString(),
            Notes = waitlistEntry.Notes
        });
    }
}
