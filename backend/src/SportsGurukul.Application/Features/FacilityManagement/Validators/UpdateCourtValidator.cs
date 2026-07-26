using FluentValidation;
using SportsGurukul.Application.Features.FacilityManagement.Commands.UpdateCourt;

namespace SportsGurukul.Application.Features.FacilityManagement.Validators;

public class UpdateCourtValidator : AbstractValidator<UpdateCourtCommand>
{
    public UpdateCourtValidator()
    {
        RuleFor(x => x.CourtId)
            .NotEmpty().WithMessage("Court ID is required.");

        RuleFor(x => x.CourtName)
            .MaximumLength(200).WithMessage("Court name must not exceed 200 characters.");
    }
}
