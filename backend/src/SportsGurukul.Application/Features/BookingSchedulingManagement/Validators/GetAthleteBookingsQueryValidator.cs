using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetAthleteBookings;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

public class GetAthleteBookingsQueryValidator : AbstractValidator<GetAthleteBookingsQuery>
{
    public GetAthleteBookingsQueryValidator()
    {
        RuleFor(x => x.AthleteId)
            .NotEmpty().WithMessage("Athlete ID is required.");
    }
}
