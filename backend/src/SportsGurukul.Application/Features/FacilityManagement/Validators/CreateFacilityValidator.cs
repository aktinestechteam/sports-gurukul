using FluentValidation;
using SportsGurukul.Application.Features.FacilityManagement.Commands.CreateFacility;

namespace SportsGurukul.Application.Features.FacilityManagement.Validators;

public class CreateFacilityValidator : AbstractValidator<CreateFacilityCommand>
{
    public CreateFacilityValidator()
    {
        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");

        RuleFor(x => x.FacilityName)
            .NotEmpty().WithMessage("Facility name is required.")
            .MaximumLength(200).WithMessage("Facility name must not exceed 200 characters.");

        RuleFor(x => x.FacilityType)
            .IsInEnum().WithMessage("Invalid facility type.");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than 0.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");
    }
}
