using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CreateBooking;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, Result<BookingDto>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IConflictDetectionService _conflictDetectionService;
    private readonly IAvailabilityService _availabilityService;
    private readonly ISchedulingEngine _schedulingEngine;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateBookingCommandHandler> _logger;

    public CreateBookingCommandHandler(
        IBookingRepository bookingRepository,
        IConflictDetectionService conflictDetectionService,
        IAvailabilityService availabilityService,
        ISchedulingEngine schedulingEngine,
        IUnitOfWork unitOfWork,
        ILogger<CreateBookingCommandHandler> logger)
    {
        _bookingRepository = bookingRepository;
        _conflictDetectionService = conflictDetectionService;
        _availabilityService = availabilityService;
        _schedulingEngine = schedulingEngine;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<BookingDto>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating booking for Academy {AcademyId}", request.AcademyId);

        if (!Enum.TryParse<BookingType>(request.BookingType, true, out var bookingType))
            return Result<BookingDto>.Failure($"Invalid booking type: {request.BookingType}");

        if (request.StartTime >= request.EndTime)
            return Result<BookingDto>.Failure("Start time must be before end time.");

        if (request.BookingDate.Date < DateTime.UtcNow.Date)
            return Result<BookingDto>.Failure("Cannot create bookings for past dates.");

        if (request.FacilityId.HasValue)
        {
            var facilityAvailable = await _availabilityService.IsFacilityAvailableAsync(
                request.FacilityId.Value, request.BookingDate, request.StartTime, request.EndTime,
                cancellationToken: cancellationToken);
            if (!facilityAvailable)
                return Result<BookingDto>.Failure("The selected facility is not available for the chosen time slot.");
        }

        if (request.CoachId.HasValue)
        {
            var coachAvailable = await _availabilityService.IsCoachAvailableAsync(
                request.CoachId.Value, request.BookingDate, request.StartTime, request.EndTime,
                cancellationToken: cancellationToken);
            if (!coachAvailable)
                return Result<BookingDto>.Failure("The selected coach is not available for the chosen time slot.");
        }

        if (request.AthleteId.HasValue)
        {
            var athleteAvailable = await _availabilityService.IsAthleteAvailableAsync(
                request.AthleteId.Value, request.BookingDate, request.StartTime, request.EndTime,
                cancellationToken: cancellationToken);
            if (!athleteAvailable)
                return Result<BookingDto>.Failure("The selected athlete already has a booking during this time slot.");
        }

        var bookingNumber = await _schedulingEngine.GenerateBookingNumberAsync(cancellationToken);

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            BookingNumber = bookingNumber,
            BookingType = bookingType,
            Status = BookingStatus.Pending,
            Title = request.Title,
            Description = request.Description,
            AcademyId = request.AcademyId,
            BranchId = request.BranchId,
            FacilityId = request.FacilityId,
            CoachId = request.CoachId,
            AthleteId = request.AthleteId,
            TrainingSessionId = request.TrainingSessionId,
            BookingDate = request.BookingDate,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Duration = (int)(request.EndTime - request.StartTime).TotalMinutes,
            ApprovalStatus = BookingApprovalStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var conflicts = await _conflictDetectionService.DetectConflictsAsync(booking, cancellationToken);
        foreach (var conflict in conflicts)
        {
            booking.Conflicts.Add(conflict);
        }

        await _bookingRepository.AddAsync(booking, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Booking created: {BookingNumber} (Id: {BookingId})", booking.BookingNumber, booking.Id);

        return Result<BookingDto>.Success(MapToDto(booking));
    }

    internal static BookingDto MapToDto(Booking booking)
    {
        return new BookingDto
        {
            Id = booking.Id,
            BookingNumber = booking.BookingNumber,
            BookingType = booking.BookingType.ToString(),
            Status = booking.Status.ToString(),
            Title = booking.Title,
            Description = booking.Description,
            AcademyId = booking.AcademyId,
            BranchId = booking.BranchId,
            FacilityId = booking.FacilityId,
            CoachId = booking.CoachId,
            AthleteId = booking.AthleteId,
            TrainingSessionId = booking.TrainingSessionId,
            BookingDate = booking.BookingDate,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            Duration = booking.Duration,
            ApprovalStatus = booking.ApprovalStatus.ToString(),
            BookingCreatorId = booking.BookingCreatorId,
            AcademyName = booking.Academy?.Name,
            FacilityName = booking.Facility?.FacilityName,
            CoachName = booking.Coach?.User?.FullName,
            AthleteName = booking.Athlete?.User?.FullName,
            CreatedAt = booking.CreatedAt,
            UpdatedAt = booking.UpdatedAt
        };
    }
}
