using FluentValidation;
using SportsGurukul.Application.Features.AcademyManagement.Commands.DeleteFacility;

namespace SportsGurukul.Application.Features.AcademyManagement.Validators;

public class DeleteFacilityValidator : AbstractValidator<DeleteFacilityCommand>
{
    public DeleteFacilityValidator()
    {
        RuleFor(x => x.FacilityId)
            .NotEmpty().WithMessage("Facility ID is required.");
    }
}
