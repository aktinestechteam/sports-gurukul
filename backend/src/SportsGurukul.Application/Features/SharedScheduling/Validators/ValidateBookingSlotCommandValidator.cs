using FluentValidation;
using SportsGurukul.Application.Features.SharedScheduling.Commands.ValidateBookingSlot;

namespace SportsGurukul.Application.Features.SharedScheduling.Validators;

public class ValidateBookingSlotCommandValidator : AbstractValidator<ValidateBookingSlotCommand>
{
    public ValidateBookingSlotCommandValidator()
    {
        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required.");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Start time is required.");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("End time is required.")
            .GreaterThan(x => x.StartTime).WithMessage("End time must be after start time.");

        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");

        RuleForEach(x => x.Resources).ChildRules(resource =>
        {
            resource.RuleFor(r => r.ResourceType)
                .NotEmpty().WithMessage("Resource type is required.");
            resource.RuleFor(r => r.ResourceId)
                .NotEmpty().WithMessage("Resource ID is required.");
        });
    }
}
