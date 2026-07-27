using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetBookingById;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

public class GetBookingByIdQueryValidator : AbstractValidator<GetBookingByIdQuery>
{
    public GetBookingByIdQueryValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty().WithMessage("Booking ID is required.");
    }
}
