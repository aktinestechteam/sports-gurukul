using FluentValidation;
using SportsGurukul.Application.Features.FacilityManagement.Commands.DeleteFacility;

namespace SportsGurukul.Application.Features.FacilityManagement.Validators;

public class DeleteFacilityValidator : AbstractValidator<DeleteFacilityCommand>
{
    public DeleteFacilityValidator()
    {
        RuleFor(x => x.FacilityId)
            .NotEmpty().WithMessage("Facility ID is required.");
    }
}
