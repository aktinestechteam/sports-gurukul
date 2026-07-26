using FluentValidation;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.GetSimilarAcademies;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Validators;

public class GetSimilarAcademiesValidator : AbstractValidator<GetSimilarAcademiesQuery>
{
    public GetSimilarAcademiesValidator()
    {
        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");

        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 20).WithMessage("Limit must be between 1 and 20.");
    }
}
