using FluentValidation;
using SportsGurukul.Application.Features.FacilityManagement.Commands.UpdateFacility;

namespace SportsGurukul.Application.Features.FacilityManagement.Validators;

public class UpdateFacilityValidator : AbstractValidator<UpdateFacilityCommand>
{
    public UpdateFacilityValidator()
    {
        RuleFor(x => x.FacilityId)
            .NotEmpty().WithMessage("Facility ID is required.");

        RuleFor(x => x.FacilityName)
            .MaximumLength(200).WithMessage("Facility name must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than 0.")
            .When(x => x.Capacity.HasValue);
    }
}
