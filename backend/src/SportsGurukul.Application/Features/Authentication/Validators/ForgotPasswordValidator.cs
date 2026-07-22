using FluentValidation;
using SportsGurukul.Application.Features.Authentication.Commands.ForgotPassword;

namespace SportsGurukul.Application.Features.Authentication.Validators;

public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");
    }
}
