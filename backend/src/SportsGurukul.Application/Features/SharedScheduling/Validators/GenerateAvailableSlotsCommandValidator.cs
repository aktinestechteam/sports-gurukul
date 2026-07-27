using FluentValidation;
using SportsGurukul.Application.Features.SharedScheduling.Commands.GenerateAvailableSlots;

namespace SportsGurukul.Application.Features.SharedScheduling.Validators;

public class GenerateAvailableSlotsCommandValidator : AbstractValidator<GenerateAvailableSlotsCommand>
{
    public GenerateAvailableSlotsCommandValidator()
    {
        RuleFor(x => x.ResourceId)
            .NotEmpty().WithMessage("Resource ID is required.");

        RuleFor(x => x.ResourceType)
            .NotEmpty().WithMessage("Resource type is required.")
            .MaximumLength(100).WithMessage("Resource type must not exceed 100 characters.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required.")
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("End date must be on or after start date.");

        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");

        RuleFor(x => x.SlotDuration)
            .Must(d => d == null || d.Value > TimeSpan.Zero).WithMessage("Slot duration must be greater than zero.");
    }
}
