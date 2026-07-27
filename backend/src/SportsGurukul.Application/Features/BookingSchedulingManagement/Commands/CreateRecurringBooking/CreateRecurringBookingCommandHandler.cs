using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CreateBooking;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CreateRecurringBooking;

public class CreateRecurringBookingCommandHandler : IRequestHandler<CreateRecurringBookingCommand, Result<BookingDto>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ISchedulingEngine _schedulingEngine;
    private readonly IRecurrenceService _recurrenceService;
    private readonly IAvailabilityService _availabilityService;
    private readonly IConflictDetectionService _conflictDetectionService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateRecurringBookingCommandHandler> _logger;

    public CreateRecurringBookingCommandHandler(
        IBookingRepository bookingRepository,
        ISchedulingEngine schedulingEngine,
        IRecurrenceService recurrenceService,
        IAvailabilityService availabilityService,
        IConflictDetectionService conflictDetectionService,
        IUnitOfWork unitOfWork,
        ILogger<CreateRecurringBookingCommandHandler> logger)
    {
        _bookingRepository = bookingRepository;
        _schedulingEngine = schedulingEngine;
        _recurrenceService = recurrenceService;
        _availabilityService = availabilityService;
        _conflictDetectionService = conflictDetectionService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<BookingDto>> Handle(CreateRecurringBookingCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating recurring booking for Academy {AcademyId}", request.AcademyId);

        if (!Enum.TryParse<BookingType>(request.BookingType, true, out var bookingType))
            return Result<BookingDto>.Failure($"Invalid booking type: {request.BookingType}");

        if (!Enum.TryParse<RecurrenceType>(request.RecurrenceType, true, out var recurrenceType))
            return Result<BookingDto>.Failure($"Invalid recurrence type: {request.RecurrenceType}");

        if (request.StartTime >= request.EndTime)
            return Result<BookingDto>.Failure("Start time must be before end time.");

        if (request.StartDate.Date < DateTime.UtcNow.Date)
            return Result<BookingDto>.Failure("Cannot create recurring bookings for past dates.");

        if (request.FacilityId.HasValue)
        {
            var available = await _availabilityService.IsFacilityAvailableAsync(
                request.FacilityId.Value, request.StartDate, request.StartTime, request.EndTime,
                cancellationToken: cancellationToken);
            if (!available)
                return Result<BookingDto>.Failure("The selected facility is not available for the chosen time slot.");
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
            BookingDate = request.StartDate,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Duration = (int)(request.EndTime - request.StartTime).TotalMinutes,
            ApprovalStatus = BookingApprovalStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var recurrence = new BookingRecurrence
        {
            Id = Guid.NewGuid(),
            BookingId = booking.Id,
            RecurrenceType = recurrenceType,
            RRule = request.RRule,
            EndDate = request.EndDate,
            OccurrenceCount = request.OccurrenceCount,
            Exceptions = request.Exceptions,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        booking.Recurrences.Add(recurrence);

        var occurrences = _recurrenceService.GenerateOccurrences(
            recurrenceType, request.StartDate, request.StartTime, request.EndTime,
            request.OccurrenceCount, request.EndDate, request.RRule, request.Exceptions);

        foreach (var occurrenceDate in occurrences)
        {
            var schedule = new BookingSchedule
            {
                Id = Guid.NewGuid(),
                BookingId = booking.Id,
                ScheduledDate = occurrenceDate,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                IsCancelled = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            booking.Schedules.Add(schedule);
        }

        var conflicts = await _conflictDetectionService.DetectConflictsAsync(booking, cancellationToken);
        foreach (var conflict in conflicts)
        {
            booking.Conflicts.Add(conflict);
        }

        await _bookingRepository.AddAsync(booking, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Recurring booking created: {BookingNumber} with {ScheduleCount} schedule instances",
            booking.BookingNumber, booking.Schedules.Count);

        return Result<BookingDto>.Success(CreateBookingCommandHandler.MapToDto(booking));
    }
}
