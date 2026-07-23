using FluentValidation;
using SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateMedicalProfile;

namespace SportsGurukul.Application.Features.AthleteManagement.Validators;

public class UpdateMedicalProfileValidator : AbstractValidator<UpdateMedicalProfileCommand>
{
    public UpdateMedicalProfileValidator()
    {
        RuleFor(x => x.AthleteId)
            .NotEmpty().WithMessage("Athlete ID is required.");

        RuleFor(x => x.MedicalConditions)
            .MaximumLength(2000).WithMessage("Medical conditions must not exceed 2000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.MedicalConditions));

        RuleFor(x => x.Allergies)
            .MaximumLength(2000).WithMessage("Allergies must not exceed 2000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Allergies));

        RuleFor(x => x.Medications)
            .MaximumLength(2000).WithMessage("Medications must not exceed 2000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Medications));

        RuleFor(x => x.BloodGroup)
            .MaximumLength(20).WithMessage("Blood group must not exceed 20 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.BloodGroup));

        RuleFor(x => x.InsuranceNumber)
            .MaximumLength(100).WithMessage("Insurance number must not exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.InsuranceNumber));

        RuleFor(x => x.DoctorName)
            .MaximumLength(200).WithMessage("Doctor name must not exceed 200 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.DoctorName));

        RuleFor(x => x.DoctorContact)
            .MaximumLength(50).WithMessage("Doctor contact must not exceed 50 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.DoctorContact));
    }
}
