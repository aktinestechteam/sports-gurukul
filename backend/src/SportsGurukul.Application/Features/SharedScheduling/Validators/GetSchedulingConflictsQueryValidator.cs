using FluentValidation;
using SportsGurukul.Application.Features.SharedScheduling.Queries.GetSchedulingConflicts;

namespace SportsGurukul.Application.Features.SharedScheduling.Validators;

public class GetSchedulingConflictsQueryValidator : AbstractValidator<GetSchedulingConflictsQuery>
{
    public GetSchedulingConflictsQueryValidator()
    {
        RuleFor(x => x.ResourceId)
            .NotEmpty().WithMessage("Resource ID is required.");

        RuleFor(x => x.ResourceType)
            .NotEmpty().WithMessage("Resource type is required.")
            .MaximumLength(100).WithMessage("Resource type must not exceed 100 characters.");
    }
}
