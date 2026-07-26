using FluentValidation;
using SportsGurukul.Application.Features.AcademyManagement.Commands.CreateAcademy;

namespace SportsGurukul.Application.Features.AcademyManagement.Validators;

public class CreateAcademyValidator : AbstractValidator<CreateAcademyCommand>
{
    public CreateAcademyValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Academy name is required.")
            .MaximumLength(200).WithMessage("Academy name must not exceed 200 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(200).WithMessage("Email must not exceed 200 characters.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone number is required.")
            .MaximumLength(50).WithMessage("Phone number must not exceed 50 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.LegalName)
            .MaximumLength(200).WithMessage("Legal name must not exceed 200 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.LegalName));

        RuleFor(x => x.RegistrationNumber)
            .MaximumLength(100).WithMessage("Registration number must not exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.RegistrationNumber));

        RuleFor(x => x.GSTNumber)
            .MaximumLength(50).WithMessage("GST number must not exceed 50 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.GSTNumber));

        RuleFor(x => x.Website)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _)).WithMessage("A valid URL is required.")
            .MaximumLength(500).WithMessage("Website URL must not exceed 500 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Website));
    }
}
