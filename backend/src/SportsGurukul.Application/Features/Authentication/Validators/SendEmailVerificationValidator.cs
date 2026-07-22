using FluentValidation;
using SportsGurukul.Application.Features.Authentication.Commands.SendEmailVerification;

namespace SportsGurukul.Application.Features.Authentication.Validators;

public class SendEmailVerificationValidator : AbstractValidator<SendEmailVerificationCommand>
{
    public SendEmailVerificationValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");
    }
}
