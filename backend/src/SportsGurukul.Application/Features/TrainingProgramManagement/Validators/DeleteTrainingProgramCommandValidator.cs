using FluentValidation;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.DeleteTrainingProgram;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Validators;

public class DeleteTrainingProgramCommandValidator : AbstractValidator<DeleteTrainingProgramCommand>
{
    public DeleteTrainingProgramCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Program ID is required.");
    }
}
