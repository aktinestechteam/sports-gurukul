using FluentValidation;
using SportsGurukul.Application.Features.CoachManagement.Commands.UpdateAvailability;

namespace SportsGurukul.Application.Features.CoachManagement.Validators;

public class UpdateAvailabilityValidator : AbstractValidator<UpdateAvailabilityCommand>
{
    public UpdateAvailabilityValidator()
    {
        RuleFor(x => x.CoachId)
            .NotEmpty().WithMessage("Coach ID is required.");

        RuleFor(x => x.WeeklySchedule)
            .MaximumLength(5000).WithMessage("Weekly schedule must not exceed 5000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.WeeklySchedule));

        RuleFor(x => x.TimeSlots)
            .MaximumLength(5000).WithMessage("Time slots must not exceed 5000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.TimeSlots));

        RuleFor(x => x.TravelDistance)
            .GreaterThanOrEqualTo(0).WithMessage("Travel distance must be non-negative.")
            .When(x => x.TravelDistance.HasValue);
    }
}
