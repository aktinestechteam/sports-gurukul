using FluentValidation;
using SportsGurukul.Application.Features.EventManagement.Commands.CancelRegistration;

namespace SportsGurukul.Application.Features.EventManagement.Validators;

public class CancelRegistrationValidator : AbstractValidator<CancelRegistrationCommand>
{
    public CancelRegistrationValidator()
    {
        RuleFor(x => x.RegistrationId)
            .NotEmpty().WithMessage("Registration ID is required.");
    }
}
