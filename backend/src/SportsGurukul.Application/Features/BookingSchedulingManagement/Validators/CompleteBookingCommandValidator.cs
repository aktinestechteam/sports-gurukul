using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CompleteBooking;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

public class CompleteBookingCommandValidator : AbstractValidator<CompleteBookingCommand>
{
    public CompleteBookingCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty().WithMessage("Booking ID is required.");
    }
}
