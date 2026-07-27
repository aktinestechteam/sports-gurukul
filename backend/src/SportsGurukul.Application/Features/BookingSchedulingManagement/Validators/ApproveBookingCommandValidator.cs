using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.ApproveBooking;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

public class ApproveBookingCommandValidator : AbstractValidator<ApproveBookingCommand>
{
    public ApproveBookingCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty().WithMessage("Booking ID is required.");

        RuleFor(x => x.ApproverUserId)
            .NotEmpty().WithMessage("Approver user ID is required.");

        RuleFor(x => x.Comments)
            .MaximumLength(1000).WithMessage("Comments must not exceed 1000 characters.");
    }
}
