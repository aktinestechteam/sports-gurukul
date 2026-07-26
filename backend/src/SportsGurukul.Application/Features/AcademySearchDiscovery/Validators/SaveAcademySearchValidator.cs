using FluentValidation;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Commands.SaveAcademySearch;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Validators;

public class SaveAcademySearchValidator : AbstractValidator<SaveAcademySearchCommand>
{
    public SaveAcademySearchValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.SearchName)
            .NotEmpty().WithMessage("Search name is required.")
            .MaximumLength(200).WithMessage("Search name must not exceed 200 characters.");
    }
}
