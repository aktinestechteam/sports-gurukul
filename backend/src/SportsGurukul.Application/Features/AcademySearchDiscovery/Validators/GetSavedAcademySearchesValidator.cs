using FluentValidation;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.GetSavedAcademySearches;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Validators;

public class GetSavedAcademySearchesValidator : AbstractValidator<GetSavedAcademySearchesQuery>
{
    public GetSavedAcademySearchesValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}
