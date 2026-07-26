using FluentValidation;
using SportsGurukul.Application.Features.FacilityManagement.Commands.DeleteCourt;

namespace SportsGurukul.Application.Features.FacilityManagement.Validators;

public class DeleteCourtValidator : AbstractValidator<DeleteCourtCommand>
{
    public DeleteCourtValidator()
    {
        RuleFor(x => x.CourtId)
            .NotEmpty().WithMessage("Court ID is required.");
    }
}
