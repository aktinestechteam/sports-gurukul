using FluentValidation;
using SportsGurukul.Application.Features.EventManagement.Commands.RescheduleSession;

namespace SportsGurukul.Application.Features.EventManagement.Validators;

public class RescheduleSessionValidator : AbstractValidator<RescheduleSessionCommand>
{
    public RescheduleSessionValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEmpty().WithMessage("Session ID is required.");

        RuleFor(x => x.SessionDate)
            .NotEmpty().WithMessage("Session date is required.");

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime).WithMessage("End time must be after start time.");
    }
}
