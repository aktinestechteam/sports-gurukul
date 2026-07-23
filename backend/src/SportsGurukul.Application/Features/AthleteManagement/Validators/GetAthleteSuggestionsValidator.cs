using FluentValidation;
using SportsGurukul.Application.Features.AthleteManagement.Queries.GetAthleteSuggestions;

namespace SportsGurukul.Application.Features.AthleteManagement.Validators;

public class GetAthleteSuggestionsValidator : AbstractValidator<GetAthleteSuggestionsQuery>
{
    public GetAthleteSuggestionsValidator()
    {
        RuleFor(x => x.Prefix)
            .NotEmpty().WithMessage("Search prefix is required.")
            .MaximumLength(100).WithMessage("Prefix must not exceed 100 characters.")
            .MinimumLength(2).WithMessage("Prefix must be at least 2 characters.");

        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 25).WithMessage("Limit must be between 1 and 25.");
    }
}
