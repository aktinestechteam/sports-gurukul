using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.SendReminder;

public class SendReminderCommandHandler : IRequestHandler<SendReminderCommand, Result<bool>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<SendReminderCommandHandler> _logger;

    public SendReminderCommandHandler(
        IBookingRepository bookingRepository,
        ILogger<SendReminderCommandHandler> logger)
    {
        _bookingRepository = bookingRepository;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(SendReminderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending reminder {ReminderId}", request.ReminderId);

        var booking = await _bookingRepository.GetByIdAsync(request.ReminderId, cancellationToken);
        if (booking is null)
            return Result<bool>.Failure("Booking not found.");

        _logger.LogInformation(
            "Reminder {ReminderId} sent via {Channel} (future notification integration point)",
            request.ReminderId, request.OverrideChannel ?? "default");

        return Result<bool>.Success(true);
    }
}
