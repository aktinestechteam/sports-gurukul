using FluentValidation;
using SportsGurukul.Application.Features.DocumentManagement.Commands.RestoreAthleteDocument;

namespace SportsGurukul.Application.Features.DocumentManagement.Validators;

public class RestoreAthleteDocumentValidator : AbstractValidator<RestoreAthleteDocumentCommand>
{
    public RestoreAthleteDocumentValidator()
    {
        RuleFor(x => x.DocumentId)
            .NotEmpty().WithMessage("Document ID is required.");
    }
}
