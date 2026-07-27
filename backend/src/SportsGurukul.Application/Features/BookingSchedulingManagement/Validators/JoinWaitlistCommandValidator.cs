using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.JoinWaitlist;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

public class JoinWaitlistCommandValidator : AbstractValidator<JoinWaitlistCommand>
{
    public JoinWaitlistCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty().WithMessage("Booking ID is required.");

        RuleFor(x => x.WaitlistUserId)
            .NotEmpty().WithMessage("Waitlist user ID is required.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes must not exceed 1000 characters.");
    }
}
