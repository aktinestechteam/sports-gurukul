using FluentValidation;
using SportsGurukul.Application.Features.EventManagement.Commands.ApproveRegistration;

namespace SportsGurukul.Application.Features.EventManagement.Validators;

public class ApproveRegistrationValidator : AbstractValidator<ApproveRegistrationCommand>
{
    public ApproveRegistrationValidator()
    {
        RuleFor(x => x.RegistrationId)
            .NotEmpty().WithMessage("Registration ID is required.");
    }
}
