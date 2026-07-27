using FluentValidation;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.ArchiveTrainingProgram;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Validators;

public class ArchiveTrainingProgramCommandValidator : AbstractValidator<ArchiveTrainingProgramCommand>
{
    public ArchiveTrainingProgramCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Program ID is required.");
    }
}
