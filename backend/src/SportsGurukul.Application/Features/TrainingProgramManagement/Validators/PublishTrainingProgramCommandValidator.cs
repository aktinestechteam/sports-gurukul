using FluentValidation;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.PublishTrainingProgram;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Validators;

public class PublishTrainingProgramCommandValidator : AbstractValidator<PublishTrainingProgramCommand>
{
    public PublishTrainingProgramCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Program ID is required.");
    }
}
