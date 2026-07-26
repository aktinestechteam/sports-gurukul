using FluentValidation;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.GetPopularSearchTerms;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Validators;

public class GetPopularSearchTermsValidator : AbstractValidator<GetPopularSearchTermsQuery>
{
    public GetPopularSearchTermsValidator()
    {
        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 20).WithMessage("Limit must be between 1 and 20.");
    }
}
