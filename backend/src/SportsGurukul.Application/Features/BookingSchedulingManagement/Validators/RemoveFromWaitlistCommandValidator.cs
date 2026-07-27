using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.RemoveFromWaitlist;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

public class RemoveFromWaitlistCommandValidator : AbstractValidator<RemoveFromWaitlistCommand>
{
    public RemoveFromWaitlistCommandValidator()
    {
        RuleFor(x => x.WaitlistEntryId)
            .NotEmpty().WithMessage("Waitlist entry ID is required.");
    }
}
