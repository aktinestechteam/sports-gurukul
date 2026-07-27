using FluentValidation;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Enrollment.EnrollAthlete;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Validators;

public class EnrollAthleteCommandValidator : AbstractValidator<EnrollAthleteCommand>
{
    public EnrollAthleteCommandValidator()
    {
        RuleFor(x => x.BatchId)
            .NotEmpty().WithMessage("Batch ID is required.");

        RuleFor(x => x.AthleteId)
            .NotEmpty().WithMessage("Athlete ID is required.");
    }
}
