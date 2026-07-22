using FluentValidation;
using SportsGurukul.Application.Features.UserManagement.Commands.UpdateUserPreference;

namespace SportsGurukul.Application.Features.UserManagement.Validators;

public class UpdateUserPreferenceValidator : AbstractValidator<UpdateUserPreferenceCommand>
{
    public UpdateUserPreferenceValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.Language)
            .MaximumLength(10).WithMessage("Language code must not exceed 10 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Language));

        RuleFor(x => x.TimeZone)
            .MaximumLength(100).WithMessage("Time zone must not exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.TimeZone));
    }
}
