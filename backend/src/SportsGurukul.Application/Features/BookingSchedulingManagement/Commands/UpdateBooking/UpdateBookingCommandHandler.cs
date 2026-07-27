using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CreateBooking;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.UpdateBooking;

public class UpdateBookingCommandHandler : IRequestHandler<UpdateBookingCommand, Result<BookingDto>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IAvailabilityService _availabilityService;
    private readonly IConflictDetectionService _conflictDetectionService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateBookingCommandHandler> _logger;

    public UpdateBookingCommandHandler(
        IBookingRepository bookingRepository,
        IAvailabilityService availabilityService,
        IConflictDetectionService conflictDetectionService,
        IUnitOfWork unitOfWork,
        ILogger<UpdateBookingCommandHandler> logger)
    {
        _bookingRepository = bookingRepository;
        _availabilityService = availabilityService;
        _conflictDetectionService = conflictDetectionService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<BookingDto>> Handle(UpdateBookingCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating booking {BookingId}", request.BookingId);

        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking is null)
            return Result<BookingDto>.Failure("Booking not found.");

        if (booking.Status != BookingStatus.Draft && booking.Status != BookingStatus.Pending)
            return Result<BookingDto>.Failure("Only draft or pending bookings can be updated.");

        if (request.Title is not null) booking.Title = request.Title;
        if (request.Description is not null) booking.Description = request.Description;

        var newDate = request.BookingDate ?? booking.BookingDate;
        var newStart = request.StartTime ?? booking.StartTime;
        var newEnd = request.EndTime ?? booking.EndTime;

        if (newStart >= newEnd)
            return Result<BookingDto>.Failure("Start time must be before end time.");

        if (request.FacilityId.HasValue || request.CoachId.HasValue)
        {
            var facilityId = request.FacilityId ?? booking.FacilityId;
            var coachId = request.CoachId ?? booking.CoachId;

            if (facilityId.HasValue)
            {
                var available = await _availabilityService.IsFacilityAvailableAsync(
                    facilityId.Value, newDate, newStart, newEnd,
                    excludeBookingId: booking.Id, cancellationToken: cancellationToken);
                if (!available)
                    return Result<BookingDto>.Failure("The selected facility is not available for the chosen time slot.");
            }

            if (coachId.HasValue)
            {
                var available = await _availabilityService.IsCoachAvailableAsync(
                    coachId.Value, newDate, newStart, newEnd,
                    excludeBookingId: booking.Id, cancellationToken: cancellationToken);
                if (!available)
                    return Result<BookingDto>.Failure("The selected coach is not available for the chosen time slot.");
            }
        }

        booking.BookingDate = newDate;
        booking.StartTime = newStart;
        booking.EndTime = newEnd;
        booking.Duration = (int)(newEnd - newStart).TotalMinutes;

        if (request.FacilityId.HasValue) booking.FacilityId = request.FacilityId;
        if (request.CoachId.HasValue) booking.CoachId = request.CoachId;
        if (request.AthleteId.HasValue) booking.AthleteId = request.AthleteId;

        booking.UpdatedAt = DateTime.UtcNow;

        _bookingRepository.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Booking updated: {BookingNumber}", booking.BookingNumber);

        return Result<BookingDto>.Success(CreateBookingCommandHandler.MapToDto(booking));
    }
}
