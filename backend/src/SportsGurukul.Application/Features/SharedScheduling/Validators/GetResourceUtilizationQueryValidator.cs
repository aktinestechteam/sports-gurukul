using FluentValidation;
using SportsGurukul.Application.Features.SharedScheduling.Queries.GetResourceUtilization;

namespace SportsGurukul.Application.Features.SharedScheduling.Validators;

public class GetResourceUtilizationQueryValidator : AbstractValidator<GetResourceUtilizationQuery>
{
    public GetResourceUtilizationQueryValidator()
    {
        RuleFor(x => x.ResourceType)
            .NotEmpty().WithMessage("Resource type is required.")
            .MaximumLength(100).WithMessage("Resource type must not exceed 100 characters.");

        RuleFor(x => x.ResourceIds)
            .NotEmpty().WithMessage("At least one resource ID is required.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required.")
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("End date must be on or after start date.");

        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");
    }
}
