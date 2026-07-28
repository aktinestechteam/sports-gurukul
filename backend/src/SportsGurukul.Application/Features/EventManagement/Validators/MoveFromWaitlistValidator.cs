using FluentValidation;
using SportsGurukul.Application.Features.EventManagement.Commands.MoveFromWaitlist;

namespace SportsGurukul.Application.Features.EventManagement.Validators;

public class MoveFromWaitlistValidator : AbstractValidator<MoveFromWaitlistCommand>
{
    public MoveFromWaitlistValidator()
    {
        RuleFor(x => x.RegistrationId)
            .NotEmpty().WithMessage("Registration ID is required.");
    }
}
