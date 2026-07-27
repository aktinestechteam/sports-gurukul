using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetBookingHistory;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

public class GetBookingHistoryQueryValidator : AbstractValidator<GetBookingHistoryQuery>
{
    public GetBookingHistoryQueryValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty().WithMessage("Booking ID is required.");
    }
}
