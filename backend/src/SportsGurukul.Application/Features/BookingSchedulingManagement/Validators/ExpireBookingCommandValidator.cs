using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.ExpireBooking;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

public class ExpireBookingCommandValidator : AbstractValidator<ExpireBookingCommand>
{
    public ExpireBookingCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty().WithMessage("Booking ID is required.");
    }
}
