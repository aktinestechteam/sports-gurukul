using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.SearchBookings;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

public class SearchBookingsQueryValidator : AbstractValidator<SearchBookingsQuery>
{
    public SearchBookingsQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(200).WithMessage("Search term must not exceed 200 characters.");

        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than zero.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than zero.")
            .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100.");
    }
}
