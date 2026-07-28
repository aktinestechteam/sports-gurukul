using FluentValidation;
using SportsGurukul.Application.Features.EventSearchDiscovery.Commands.DeleteSavedSearch;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Validators;

public class DeleteSavedSearchValidator : AbstractValidator<DeleteSavedSearchCommand>
{
    public DeleteSavedSearchValidator()
    {
        RuleFor(x => x.SavedSearchId)
            .NotEmpty().WithMessage("Saved search ID is required.");
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}
