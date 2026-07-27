using FluentValidation;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.UpdateTrainingProgram;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Validators;

public class UpdateTrainingProgramCommandValidator : AbstractValidator<UpdateTrainingProgramCommand>
{
    public UpdateTrainingProgramCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Program ID is required.");

        RuleFor(x => x.ProgramName)
            .NotEmpty().WithMessage("Program name is required.")
            .MaximumLength(200).WithMessage("Program name must not exceed 200 characters.");

        RuleFor(x => x.SportId)
            .NotEmpty().WithMessage("Sport ID is required.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

        RuleFor(x => x.DifficultyLevel)
            .IsInEnum().WithMessage("Invalid difficulty level.");

        RuleFor(x => x.MinimumAge)
            .InclusiveBetween(5, 100).WithMessage("Minimum age must be between 5 and 100.");

        RuleFor(x => x.MaximumAge)
            .InclusiveBetween(5, 100).WithMessage("Maximum age must be between 5 and 100.")
            .GreaterThanOrEqualTo(x => x.MinimumAge).WithMessage("Maximum age must be greater than or equal to minimum age.");

        RuleFor(x => x.DurationWeeks)
            .InclusiveBetween(1, 52).WithMessage("Duration must be between 1 and 52 weeks.");

        RuleFor(x => x.Capacity)
            .InclusiveBetween(1, 1000).WithMessage("Capacity must be between 1 and 1000.");
    }
}
