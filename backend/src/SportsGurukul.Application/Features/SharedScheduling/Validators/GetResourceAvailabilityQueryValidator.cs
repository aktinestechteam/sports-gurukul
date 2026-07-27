using FluentValidation;
using SportsGurukul.Application.Features.SharedScheduling.Queries.GetResourceAvailability;

namespace SportsGurukul.Application.Features.SharedScheduling.Validators;

public class GetResourceAvailabilityQueryValidator : AbstractValidator<GetResourceAvailabilityQuery>
{
    public GetResourceAvailabilityQueryValidator()
    {
        RuleFor(x => x.ResourceId)
            .NotEmpty().WithMessage("Resource ID is required.");

        RuleFor(x => x.ResourceType)
            .NotEmpty().WithMessage("Resource type is required.")
            .MaximumLength(100).WithMessage("Resource type must not exceed 100 characters.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required.");

        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");
    }
}
