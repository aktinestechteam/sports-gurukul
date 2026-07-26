using FluentValidation;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Commands.RecordAcademySearch;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Validators;

public class RecordAcademySearchValidator : AbstractValidator<RecordAcademySearchCommand>
{
    public RecordAcademySearchValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.SearchTerm)
            .NotEmpty().WithMessage("Search term is required.")
            .MaximumLength(500).WithMessage("Search term must not exceed 500 characters.");
    }
}
