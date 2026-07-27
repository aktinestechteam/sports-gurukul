using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.ConfirmBooking;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

public class ConfirmBookingCommandValidator : AbstractValidator<ConfirmBookingCommand>
{
    public ConfirmBookingCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty().WithMessage("Booking ID is required.");
    }
}
