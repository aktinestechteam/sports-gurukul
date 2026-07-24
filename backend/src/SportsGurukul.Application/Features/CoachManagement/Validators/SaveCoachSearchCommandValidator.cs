using FluentValidation;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.SaveCoachSearch;

public class SaveCoachSearchCommandValidator : AbstractValidator<SaveCoachSearchCommand>
{
    public SaveCoachSearchCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Search name is required.")
            .MaximumLength(100).WithMessage("Search name must not exceed 100 characters.");

        RuleFor(x => x.FiltersJson)
            .NotEmpty().WithMessage("Filters are required.")
            .MaximumLength(4000).WithMessage("Filters must not exceed 4000 characters.");
    }
}
