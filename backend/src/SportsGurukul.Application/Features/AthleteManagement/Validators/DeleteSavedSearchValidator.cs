using FluentValidation;
using SportsGurukul.Application.Features.AthleteManagement.Commands.DeleteSavedSearch;

namespace SportsGurukul.Application.Features.AthleteManagement.Validators;

public class DeleteSavedSearchValidator : AbstractValidator<DeleteSavedSearchCommand>
{
    public DeleteSavedSearchValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Saved search ID is required.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}
