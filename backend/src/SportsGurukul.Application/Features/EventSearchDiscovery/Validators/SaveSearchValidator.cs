using FluentValidation;
using SportsGurukul.Application.Features.EventSearchDiscovery.Commands.SaveSearch;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Validators;

public class SaveSearchValidator : AbstractValidator<SaveSearchCommand>
{
    public SaveSearchValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
        RuleFor(x => x.SearchName)
            .NotEmpty().WithMessage("Search name is required.")
            .MaximumLength(200).WithMessage("Search name must not exceed 200 characters.");
    }
}
