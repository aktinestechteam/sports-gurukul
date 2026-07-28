using FluentValidation;
using SportsGurukul.Application.Features.EventSearchDiscovery.Queries.SearchEvents;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Validators;

public class SearchEventsValidator : AbstractValidator<SearchEventsQuery>
{
    public SearchEventsValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum price must be at least 0.")
            .When(x => x.MinPrice.HasValue);
        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Maximum price must be at least 0.")
            .When(x => x.MaxPrice.HasValue);
        RuleFor(x => x)
            .Must(x => !x.DateFrom.HasValue || !x.DateTo.HasValue || x.DateFrom <= x.DateTo)
            .WithMessage("Date from must be less than or equal to date to.")
            .When(x => x.DateFrom.HasValue && x.DateTo.HasValue);
    }
}
