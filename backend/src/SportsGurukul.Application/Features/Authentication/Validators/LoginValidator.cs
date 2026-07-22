using FluentValidation;
using SportsGurukul.Application.Features.Authentication.Commands.LoginUser;

namespace SportsGurukul.Application.Features.Authentication.Validators;

public class LoginValidator : AbstractValidator<LoginUserCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
