using FluentValidation;
using SportsGurukul.Application.Features.AthleteManagement.Commands.RecordRecentSearch;

namespace SportsGurukul.Application.Features.AthleteManagement.Validators;

public class RecordRecentSearchValidator : AbstractValidator<RecordRecentSearchCommand>
{
    public RecordRecentSearchValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.QueryText)
            .MaximumLength(500).WithMessage("Query text must not exceed 500 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.QueryText));

        RuleFor(x => x.FiltersJson)
            .MaximumLength(4000).WithMessage("Filters must not exceed 4000 characters.");
    }
}
