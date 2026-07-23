using FluentValidation;
using SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateEmergencyContact;

namespace SportsGurukul.Application.Features.AthleteManagement.Validators;

public class UpdateEmergencyContactValidator : AbstractValidator<UpdateEmergencyContactCommand>
{
    public UpdateEmergencyContactValidator()
    {
        RuleFor(x => x.AthleteId)
            .NotEmpty().WithMessage("Athlete ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone is required.")
            .MaximumLength(50).WithMessage("Phone must not exceed 50 characters.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email address.")
            .MaximumLength(200).WithMessage("Email must not exceed 200 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}
