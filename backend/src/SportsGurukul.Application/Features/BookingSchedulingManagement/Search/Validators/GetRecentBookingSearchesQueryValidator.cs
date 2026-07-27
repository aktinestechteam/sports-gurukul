using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Queries.GetRecentBookingSearches;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Validators;

public class GetRecentBookingSearchesQueryValidator : AbstractValidator<GetRecentBookingSearchesQuery>
{
    public GetRecentBookingSearchesQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.Limit)
            .GreaterThan(0).WithMessage("Limit must be greater than zero.")
            .LessThanOrEqualTo(50).WithMessage("Limit must not exceed 50.");
    }
}
