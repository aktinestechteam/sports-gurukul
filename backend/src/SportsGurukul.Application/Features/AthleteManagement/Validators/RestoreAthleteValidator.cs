using FluentValidation;
using SportsGurukul.Application.Features.AthleteManagement.Commands.RestoreAthlete;

namespace SportsGurukul.Application.Features.AthleteManagement.Validators;

public class RestoreAthleteValidator : AbstractValidator<RestoreAthleteCommand>
{
    public RestoreAthleteValidator()
    {
        RuleFor(x => x.AthleteId)
            .NotEmpty().WithMessage("Athlete ID is required.");
    }
}
