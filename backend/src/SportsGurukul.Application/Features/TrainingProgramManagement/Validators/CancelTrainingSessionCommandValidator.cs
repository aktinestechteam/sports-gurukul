using FluentValidation;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.CancelTrainingSession;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Validators;

public class CancelTrainingSessionCommandValidator : AbstractValidator<CancelTrainingSessionCommand>
{
    public CancelTrainingSessionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Session ID is required.");
    }
}
