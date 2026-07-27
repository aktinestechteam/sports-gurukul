using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetUpcomingBookings;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

public class GetUpcomingBookingsQueryValidator : AbstractValidator<GetUpcomingBookingsQuery>
{
    public GetUpcomingBookingsQueryValidator()
    {
        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");

        RuleFor(x => x.DaysAhead)
            .GreaterThan(0).WithMessage("Days ahead must be greater than zero.")
            .LessThanOrEqualTo(90).WithMessage("Days ahead must not exceed 90.");
    }
}
