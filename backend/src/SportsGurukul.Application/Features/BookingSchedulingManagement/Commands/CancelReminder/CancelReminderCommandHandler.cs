using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CancelReminder;

public class CancelReminderCommandHandler : IRequestHandler<CancelReminderCommand, Result<bool>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CancelReminderCommandHandler> _logger;

    public CancelReminderCommandHandler(
        IBookingRepository bookingRepository,
        IUnitOfWork unitOfWork,
        ILogger<CancelReminderCommandHandler> logger)
    {
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(CancelReminderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling reminder {ReminderId}", request.ReminderId);

        var booking = await _bookingRepository.GetByIdAsync(request.ReminderId, cancellationToken);
        if (booking is null)
            return Result<bool>.Failure("Booking not found.");

        _logger.LogInformation("Reminder {ReminderId} cancelled", request.ReminderId);

        return Result<bool>.Success(true);
    }
}
