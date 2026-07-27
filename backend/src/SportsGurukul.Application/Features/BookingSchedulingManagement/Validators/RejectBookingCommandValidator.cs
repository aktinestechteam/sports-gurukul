using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.RejectBooking;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

public class RejectBookingCommandValidator : AbstractValidator<RejectBookingCommand>
{
    public RejectBookingCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty().WithMessage("Booking ID is required.");

        RuleFor(x => x.Reason)
            .MaximumLength(500).WithMessage("Reason must not exceed 500 characters.");
    }
}
