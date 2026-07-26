using FluentValidation;
using SportsGurukul.Application.Features.AcademyManagement.Commands.UpdateFacility;

namespace SportsGurukul.Application.Features.AcademyManagement.Validators;

public class UpdateFacilityValidator : AbstractValidator<UpdateFacilityCommand>
{
    public UpdateFacilityValidator()
    {
        RuleFor(x => x.FacilityId)
            .NotEmpty().WithMessage("Facility ID is required.");

        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");

        RuleFor(x => x.FacilityName)
            .MaximumLength(200).WithMessage("Facility name must not exceed 200 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.FacilityName));

        RuleFor(x => x.FacilityType)
            .IsInEnum().WithMessage("A valid facility type is required.")
            .When(x => x.FacilityType.HasValue);

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than 0.")
            .When(x => x.Capacity.HasValue);

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
    }
}
