using FluentValidation;
using SportsGurukul.Application.Features.DocumentManagement.Queries.GetAthleteDocuments;

namespace SportsGurukul.Application.Features.DocumentManagement.Validators;

public class GetAthleteDocumentsValidator : AbstractValidator<GetAthleteDocumentsQuery>
{
    public GetAthleteDocumentsValidator()
    {
        RuleFor(x => x.AthleteId)
            .NotEmpty().WithMessage("Athlete ID is required.");
    }
}
