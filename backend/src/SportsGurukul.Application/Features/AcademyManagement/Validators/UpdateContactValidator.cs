using FluentValidation;
using SportsGurukul.Application.Features.AcademyManagement.Commands.UpdateContact;

namespace SportsGurukul.Application.Features.AcademyManagement.Validators;

public class UpdateContactValidator : AbstractValidator<UpdateContactCommand>
{
    public UpdateContactValidator()
    {
        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");

        RuleFor(x => x.PrimaryPhone)
            .MaximumLength(50).WithMessage("Primary phone must not exceed 50 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.PrimaryPhone));

        RuleFor(x => x.PrimaryEmail)
            .EmailAddress().WithMessage("A valid primary email address is required.")
            .MaximumLength(200).WithMessage("Primary email must not exceed 200 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.PrimaryEmail));

        RuleFor(x => x.SecondaryPhone)
            .MaximumLength(50).WithMessage("Secondary phone must not exceed 50 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.SecondaryPhone));

        RuleFor(x => x.SecondaryEmail)
            .EmailAddress().WithMessage("A valid secondary email address is required.")
            .MaximumLength(200).WithMessage("Secondary email must not exceed 200 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.SecondaryEmail));

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Address must not exceed 500 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Address));

        RuleFor(x => x.Country)
            .MaximumLength(100).WithMessage("Country must not exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Country));

        RuleFor(x => x.State)
            .MaximumLength(100).WithMessage("State must not exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.State));

        RuleFor(x => x.City)
            .MaximumLength(100).WithMessage("City must not exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.City));

        RuleFor(x => x.PostalCode)
            .MaximumLength(20).WithMessage("Postal code must not exceed 20 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.PostalCode));

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.")
            .When(x => x.Latitude.HasValue);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.")
            .When(x => x.Longitude.HasValue);
    }
}
