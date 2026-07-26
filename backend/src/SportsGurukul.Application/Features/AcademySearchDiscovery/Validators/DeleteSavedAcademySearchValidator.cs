using FluentValidation;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Commands.DeleteSavedAcademySearch;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Validators;

public class DeleteSavedAcademySearchValidator : AbstractValidator<DeleteSavedAcademySearchCommand>
{
    public DeleteSavedAcademySearchValidator()
    {
        RuleFor(x => x.SearchId)
            .NotEmpty().WithMessage("Search ID is required.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}
