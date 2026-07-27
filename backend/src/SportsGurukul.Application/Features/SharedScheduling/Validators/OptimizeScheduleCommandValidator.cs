using FluentValidation;
using SportsGurukul.Application.Features.SharedScheduling.Commands.OptimizeSchedule;

namespace SportsGurukul.Application.Features.SharedScheduling.Validators;

public class OptimizeScheduleCommandValidator : AbstractValidator<OptimizeScheduleCommand>
{
    public OptimizeScheduleCommandValidator()
    {
        RuleFor(x => x.ResourceType)
            .NotEmpty().WithMessage("Resource type is required.")
            .MaximumLength(100).WithMessage("Resource type must not exceed 100 characters.");

        RuleFor(x => x.ResourceIds)
            .NotEmpty().WithMessage("At least one resource ID is required.");

        RuleFor(x => x.PreferredDate)
            .NotEmpty().WithMessage("Preferred date is required.");

        RuleFor(x => x.Duration)
            .GreaterThan(TimeSpan.Zero).WithMessage("Duration must be greater than zero.");

        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");
    }
}
