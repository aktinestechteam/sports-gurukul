using FluentValidation;
using SportsGurukul.Application.Features.AthleteManagement.Commands.CreateSavedSearch;

namespace SportsGurukul.Application.Features.AthleteManagement.Validators;

public class CreateSavedSearchValidator : AbstractValidator<CreateSavedSearchCommand>
{
    public CreateSavedSearchValidator()
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
