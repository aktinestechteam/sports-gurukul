using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.ScheduleReminder;

public class ScheduleReminderCommandHandler : IRequestHandler<ScheduleReminderCommand, Result<ReminderDto>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ScheduleReminderCommandHandler> _logger;

    public ScheduleReminderCommandHandler(
        IBookingRepository bookingRepository,
        IUnitOfWork unitOfWork,
        ILogger<ScheduleReminderCommandHandler> logger)
    {
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ReminderDto>> Handle(ScheduleReminderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Scheduling reminder for booking {BookingId}", request.BookingId);

        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking is null)
            return Result<ReminderDto>.Failure("Booking not found.");

        var scheduledAt = booking.BookingDate.Add(booking.StartTime)
            .AddMinutes(-request.ReminderMinutesBefore);

        var reminder = new BookingReminder
        {
            Id = Guid.NewGuid(),
            BookingId = request.BookingId,
            ReminderMinutesBefore = request.ReminderMinutesBefore,
            ScheduledAt = scheduledAt,
            IsSent = false,
            Channel = request.Channel,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _bookingRepository.AddAsync(booking, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Reminder scheduled for booking {BookingNumber} at {ScheduledAt}",
            booking.BookingNumber, scheduledAt);

        return Result<ReminderDto>.Success(new ReminderDto
        {
            Id = reminder.Id,
            BookingId = reminder.BookingId,
            ReminderMinutesBefore = reminder.ReminderMinutesBefore,
            ScheduledAt = reminder.ScheduledAt,
            IsSent = reminder.IsSent,
            Channel = reminder.Channel,
            Notes = reminder.Notes
        });
    }
}
