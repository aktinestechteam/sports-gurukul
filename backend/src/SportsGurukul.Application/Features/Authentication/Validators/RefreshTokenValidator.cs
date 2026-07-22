using FluentValidation;
using SportsGurukul.Application.Features.Authentication.Commands.RefreshToken;

namespace SportsGurukul.Application.Features.Authentication.Validators;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.");
    }
}
