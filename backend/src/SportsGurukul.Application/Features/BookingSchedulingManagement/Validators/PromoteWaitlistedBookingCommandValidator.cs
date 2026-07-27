using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.PromoteWaitlistedBooking;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

public class PromoteWaitlistedBookingCommandValidator : AbstractValidator<PromoteWaitlistedBookingCommand>
{
    public PromoteWaitlistedBookingCommandValidator()
    {
        RuleFor(x => x.WaitlistEntryId)
            .NotEmpty().WithMessage("Waitlist entry ID is required.");
    }
}
