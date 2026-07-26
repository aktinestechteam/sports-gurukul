using FluentValidation;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.GetAcademySuggestions;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Validators;

public class GetAcademySuggestionsValidator : AbstractValidator<GetAcademySuggestionsQuery>
{
    public GetAcademySuggestionsValidator()
    {
        RuleFor(x => x.Prefix)
            .NotEmpty().WithMessage("Search prefix is required.")
            .MaximumLength(200).WithMessage("Prefix must not exceed 200 characters.");

        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 20).WithMessage("Limit must be between 1 and 20.");
    }
}
