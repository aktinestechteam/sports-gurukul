using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetBookingConflicts;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

public class GetBookingConflictsQueryValidator : AbstractValidator<GetBookingConflictsQuery>
{
    public GetBookingConflictsQueryValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty().WithMessage("Booking ID is required.");
    }
}
