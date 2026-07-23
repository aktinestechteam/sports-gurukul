using FluentValidation;
using SportsGurukul.Application.Features.AthleteManagement.Queries.GetSavedSearches;

namespace SportsGurukul.Application.Features.AthleteManagement.Validators;

public class GetSavedSearchesValidator : AbstractValidator<GetSavedSearchesQuery>
{
    public GetSavedSearchesValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}
