using FluentValidation;
using SportsGurukul.Application.Features.Authentication.Commands.VerifyEmail;

namespace SportsGurukul.Application.Features.Authentication.Validators;

public class VerifyEmailValidator : AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Verification token is required.");
    }
}
