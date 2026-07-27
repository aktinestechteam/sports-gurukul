using FluentValidation;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.RestoreTrainingProgram;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Validators;

public class RestoreTrainingProgramCommandValidator : AbstractValidator<RestoreTrainingProgramCommand>
{
    public RestoreTrainingProgramCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Program ID is required.");
    }
}
