using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetCoachBookings;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

public class GetCoachBookingsQueryValidator : AbstractValidator<GetCoachBookingsQuery>
{
    public GetCoachBookingsQueryValidator()
    {
        RuleFor(x => x.CoachId)
            .NotEmpty().WithMessage("Coach ID is required.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required.");
    }
}
