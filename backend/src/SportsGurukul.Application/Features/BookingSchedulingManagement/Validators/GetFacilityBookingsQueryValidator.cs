using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetFacilityBookings;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

public class GetFacilityBookingsQueryValidator : AbstractValidator<GetFacilityBookingsQuery>
{
    public GetFacilityBookingsQueryValidator()
    {
        RuleFor(x => x.FacilityId)
            .NotEmpty().WithMessage("Facility ID is required.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required.");
    }
}
