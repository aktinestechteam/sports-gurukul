using FluentValidation;
using SportsGurukul.Application.Features.EventSearchDiscovery.Queries.Autocomplete;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Validators;

public class AutocompleteValidator : AbstractValidator<AutocompleteQuery>
{
    public AutocompleteValidator()
    {
        RuleFor(x => x.Prefix)
            .NotEmpty().WithMessage("Prefix is required.")
            .MaximumLength(100).WithMessage("Prefix must not exceed 100 characters.");
        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 20).WithMessage("Limit must be between 1 and 20.");
    }
}
