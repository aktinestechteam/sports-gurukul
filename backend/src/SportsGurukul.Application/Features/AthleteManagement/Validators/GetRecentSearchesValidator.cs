using FluentValidation;
using SportsGurukul.Application.Features.AthleteManagement.Queries.GetRecentSearches;

namespace SportsGurukul.Application.Features.AthleteManagement.Validators;

public class GetRecentSearchesValidator : AbstractValidator<GetRecentSearchesQuery>
{
    public GetRecentSearchesValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 50).WithMessage("Limit must be between 1 and 50.");
    }
}
