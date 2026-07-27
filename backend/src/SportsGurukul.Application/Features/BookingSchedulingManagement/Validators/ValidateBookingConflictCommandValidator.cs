using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.ValidateBookingConflict;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

public class ValidateBookingConflictCommandValidator : AbstractValidator<ValidateBookingConflictCommand>
{
    public ValidateBookingConflictCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty().WithMessage("Booking ID is required.");
    }
}
