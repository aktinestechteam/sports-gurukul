using FluentValidation;
using SportsGurukul.Application.Features.EventManagement.Commands.ScheduleEvent;

namespace SportsGurukul.Application.Features.EventManagement.Validators;

public class ScheduleEventValidator : AbstractValidator<ScheduleEventCommand>
{
    public ScheduleEventValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("Event ID is required.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required.")
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.");

        RuleFor(x => x.RegistrationOpenDate)
            .NotEmpty().WithMessage("Registration open date is required.");

        RuleFor(x => x.RegistrationCloseDate)
            .NotEmpty().WithMessage("Registration close date is required.")
            .LessThan(x => x.StartDate).WithMessage("Registration close date must be before start date.");
    }
}
