using FluentValidation;
using SportsGurukul.Application.Features.SharedScheduling.Commands.ResolveSchedulingConflict;

namespace SportsGurukul.Application.Features.SharedScheduling.Validators;

public class ResolveSchedulingConflictCommandValidator : AbstractValidator<ResolveSchedulingConflictCommand>
{
    public ResolveSchedulingConflictCommandValidator()
    {
        RuleFor(x => x.ConflictId)
            .NotEmpty().WithMessage("Conflict ID is required.");

        RuleFor(x => x.ResolutionNotes)
            .NotEmpty().WithMessage("Resolution notes are required.")
            .MaximumLength(1000).WithMessage("Resolution notes must not exceed 1000 characters.");
    }
}
