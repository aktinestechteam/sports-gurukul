using FluentValidation;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.AssignFacility;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Validators;

public class AssignFacilityCommandValidator : AbstractValidator<AssignFacilityCommand>
{
    public AssignFacilityCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Session ID is required.");

        RuleFor(x => x.FacilityId)
            .NotEmpty().WithMessage("Facility ID is required.");
    }
}
