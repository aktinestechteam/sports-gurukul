using FluentValidation;
using SportsGurukul.Application.Features.FacilityManagement.Commands.AddCourt;

namespace SportsGurukul.Application.Features.FacilityManagement.Validators;

public class AddCourtValidator : AbstractValidator<AddCourtCommand>
{
    public AddCourtValidator()
    {
        RuleFor(x => x.FacilityId)
            .NotEmpty().WithMessage("Facility ID is required.");

        RuleFor(x => x.CourtNumber)
            .NotEmpty().WithMessage("Court number is required.")
            .MaximumLength(20).WithMessage("Court number must not exceed 20 characters.");

        RuleFor(x => x.CourtName)
            .NotEmpty().WithMessage("Court name is required.")
            .MaximumLength(200).WithMessage("Court name must not exceed 200 characters.");
    }
}
