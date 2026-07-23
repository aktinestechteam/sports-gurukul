using FluentValidation;
using SportsGurukul.Application.Features.AthleteManagement.Commands.DeleteAthlete;

namespace SportsGurukul.Application.Features.AthleteManagement.Validators;

public class DeleteAthleteValidator : AbstractValidator<DeleteAthleteCommand>
{
    public DeleteAthleteValidator()
    {
        RuleFor(x => x.AthleteId)
            .NotEmpty().WithMessage("Athlete ID is required.");
    }
}
