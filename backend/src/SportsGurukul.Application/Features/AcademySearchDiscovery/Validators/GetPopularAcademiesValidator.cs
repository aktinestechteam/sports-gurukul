using FluentValidation;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.GetPopularAcademies;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Validators;

public class GetPopularAcademiesValidator : AbstractValidator<GetPopularAcademiesQuery>
{
    public GetPopularAcademiesValidator()
    {
        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 50).WithMessage("Limit must be between 1 and 50.");
    }
}
