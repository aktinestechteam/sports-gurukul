using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.RescheduleBooking;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

public class RescheduleBookingCommandValidator : AbstractValidator<RescheduleBookingCommand>
{
    public RescheduleBookingCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty().WithMessage("Booking ID is required.");

        RuleFor(x => x.NewDate)
            .NotEmpty().WithMessage("New date is required.");

        RuleFor(x => x.NewStartTime)
            .NotEmpty().WithMessage("New start time is required.");

        RuleFor(x => x.NewEndTime)
            .NotEmpty().WithMessage("New end time is required.");

        RuleFor(x => x)
            .Must(x => x.NewStartTime < x.NewEndTime)
            .WithMessage("New start time must be before new end time.");

        RuleFor(x => x.Reason)
            .MaximumLength(500).WithMessage("Reason must not exceed 500 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes must not exceed 1000 characters.");
    }
}
