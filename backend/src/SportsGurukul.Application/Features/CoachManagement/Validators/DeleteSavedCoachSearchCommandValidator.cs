using FluentValidation;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.DeleteSavedCoachSearch;

public class DeleteSavedCoachSearchCommandValidator : AbstractValidator<DeleteSavedCoachSearchCommand>
{
    public DeleteSavedCoachSearchCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Search ID is required.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}
