using FluentValidation;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.GetRecentAcademySearches;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Validators;

public class GetRecentAcademySearchesValidator : AbstractValidator<GetRecentAcademySearchesQuery>
{
    public GetRecentAcademySearchesValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 50).WithMessage("Limit must be between 1 and 50.");
    }
}
