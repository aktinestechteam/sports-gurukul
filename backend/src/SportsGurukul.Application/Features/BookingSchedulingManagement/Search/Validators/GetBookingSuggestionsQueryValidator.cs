using FluentValidation;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Queries.GetBookingSuggestions;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Validators;

public class GetBookingSuggestionsQueryValidator : AbstractValidator<GetBookingSuggestionsQuery>
{
    public GetBookingSuggestionsQueryValidator()
    {
        RuleFor(x => x.Prefix)
            .NotEmpty().WithMessage("Prefix is required.")
            .MinimumLength(2).WithMessage("Prefix must be at least 2 characters.")
            .MaximumLength(100).WithMessage("Prefix must not exceed 100 characters.");

        RuleFor(x => x.Limit)
            .GreaterThan(0).WithMessage("Limit must be greater than zero.")
            .LessThanOrEqualTo(50).WithMessage("Limit must not exceed 50.");
    }
}
