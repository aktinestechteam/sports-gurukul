using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Queries.AdvancedSearchBookings;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Validators;

public class AdvancedSearchBookingsQueryValidator : AbstractValidator<AdvancedSearchBookingsQuery>
{
    public AdvancedSearchBookingsQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(200).WithMessage("Search term must not exceed 200 characters.");

        RuleFor(x => x.BookingNumber)
            .MaximumLength(50).WithMessage("Booking number must not exceed 50 characters.");

        RuleFor(x => x.Title)
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.DateTo)
            .GreaterThanOrEqualTo(x => x.DateFrom)
            .When(x => x.DateFrom.HasValue && x.DateTo.HasValue)
            .WithMessage("DateTo must be after DateFrom.");

        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than zero.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than zero.")
            .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100.");
    }
}
