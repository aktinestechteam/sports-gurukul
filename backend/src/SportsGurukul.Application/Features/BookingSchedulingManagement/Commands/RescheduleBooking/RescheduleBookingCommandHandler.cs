using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CreateBooking;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.RescheduleBooking;

public class RescheduleBookingCommandHandler : IRequestHandler<RescheduleBookingCommand, Result<BookingDto>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IAvailabilityService _availabilityService;
    private readonly IConflictDetectionService _conflictDetectionService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RescheduleBookingCommandHandler> _logger;

    public RescheduleBookingCommandHandler(
        IBookingRepository bookingRepository,
        IAvailabilityService availabilityService,
        IConflictDetectionService conflictDetectionService,
        IUnitOfWork unitOfWork,
        ILogger<RescheduleBookingCommandHandler> logger)
    {
        _bookingRepository = bookingRepository;
        _availabilityService = availabilityService;
        _conflictDetectionService = conflictDetectionService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<BookingDto>> Handle(RescheduleBookingCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Rescheduling booking {BookingId}", request.BookingId);

        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking is null)
            return Result<BookingDto>.Failure("Booking not found.");

        if (booking.Status != BookingStatus.Pending && booking.Status != BookingStatus.Confirmed)
            return Result<BookingDto>.Failure("Only pending or confirmed bookings can be rescheduled.");

        if (request.NewStartTime >= request.NewEndTime)
            return Result<BookingDto>.Failure("New start time must be before new end time.");

        if (request.NewDate.Date < DateTime.UtcNow.Date)
            return Result<BookingDto>.Failure("Cannot reschedule to a past date.");

        if (booking.FacilityId.HasValue)
        {
            var facilityAvailable = await _availabilityService.IsFacilityAvailableAsync(
                booking.FacilityId.Value, request.NewDate, request.NewStartTime, request.NewEndTime,
                excludeBookingId: booking.Id, cancellationToken: cancellationToken);
            if (!facilityAvailable)
                return Result<BookingDto>.Failure("The facility is not available for the new time slot.");
        }

        if (booking.CoachId.HasValue)
        {
            var coachAvailable = await _availabilityService.IsCoachAvailableAsync(
                booking.CoachId.Value, request.NewDate, request.NewStartTime, request.NewEndTime,
                excludeBookingId: booking.Id, cancellationToken: cancellationToken);
            if (!coachAvailable)
                return Result<BookingDto>.Failure("The coach is not available for the new time slot.");
        }

        var reschedule = new BookingReschedule
        {
            Id = Guid.NewGuid(),
            BookingId = booking.Id,
            RequestedById = booking.BookingCreatorId ?? Guid.Empty,
            OriginalDate = booking.BookingDate,
            OriginalStartTime = booking.StartTime,
            OriginalEndTime = booking.EndTime,
            NewDate = request.NewDate,
            NewStartTime = request.NewStartTime,
            NewEndTime = request.NewEndTime,
            Reason = request.Reason,
            IsApproved = true,
            ApprovedOn = DateTime.UtcNow,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        booking.BookingDate = request.NewDate;
        booking.StartTime = request.NewStartTime;
        booking.EndTime = request.NewEndTime;
        booking.Duration = (int)(request.NewEndTime - request.NewStartTime).TotalMinutes;
        booking.UpdatedAt = DateTime.UtcNow;

        _bookingRepository.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Booking rescheduled: {BookingNumber} to {NewDate}", booking.BookingNumber, request.NewDate);

        return Result<BookingDto>.Success(CreateBookingCommandHandler.MapToDto(booking));
    }
}
