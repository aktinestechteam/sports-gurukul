using FluentValidation;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.CompleteTrainingSession;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Validators;

public class CompleteTrainingSessionCommandValidator : AbstractValidator<CompleteTrainingSessionCommand>
{
    public CompleteTrainingSessionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Session ID is required.");
    }
}
