using FluentValidation;
using SportsGurukul.Application.Features.EventManagement.Commands.RejectRegistration;

namespace SportsGurukul.Application.Features.EventManagement.Validators;

public class RejectRegistrationValidator : AbstractValidator<RejectRegistrationCommand>
{
    public RejectRegistrationValidator()
    {
        RuleFor(x => x.RegistrationId)
            .NotEmpty().WithMessage("Registration ID is required.");
    }
}
