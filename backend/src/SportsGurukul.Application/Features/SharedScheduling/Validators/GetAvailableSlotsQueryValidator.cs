using FluentValidation;
using SportsGurukul.Application.Features.SharedScheduling.Queries.GetAvailableSlots;

namespace SportsGurukul.Application.Features.SharedScheduling.Validators;

public class GetAvailableSlotsQueryValidator : AbstractValidator<GetAvailableSlotsQuery>
{
    public GetAvailableSlotsQueryValidator()
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

        RuleFor(x => x.SlotDuration)
            .Must(d => d == null || d.Value > TimeSpan.Zero).WithMessage("Slot duration must be greater than zero.");
    }
}
