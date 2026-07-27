using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.SharedScheduling.Engine;
using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Commands.ValidateBookingSlot;

public class ValidateBookingSlotCommandHandler
    : IRequestHandler<ValidateBookingSlotCommand, Result<bool>>
{
    private readonly ISchedulingEngine _schedulingEngine;
    private readonly ILogger<ValidateBookingSlotCommandHandler> _logger;

    public ValidateBookingSlotCommandHandler(
        ISchedulingEngine schedulingEngine,
        ILogger<ValidateBookingSlotCommandHandler> logger)
    {
        _schedulingEngine = schedulingEngine;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(
        ValidateBookingSlotCommand request, CancellationToken cancellationToken)
    {
        var slot = new TimeSlot
        {
            Date = request.Date.Date,
            StartTime = request.StartTime,
            EndTime = request.EndTime
        };

        var context = new SchedulingContext
        {
            AcademyId = request.AcademyId,
            BranchId = request.BranchId,
            TimeZoneId = request.TimeZoneId ?? "UTC"
        };

        var isValid = await _schedulingEngine.ValidateSlotAsync(
            slot, context, request.Resources, cancellationToken);

        _logger.LogInformation(
            "Slot validation for {Date} {Start}-{End}: {IsValid}",
            request.Date.Date, request.StartTime, request.EndTime, isValid);

        return Result<bool>.Success(isValid);
    }
}
