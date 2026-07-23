using FluentValidation;
using SportsGurukul.Application.Features.DocumentManagement.Commands.DeleteAthleteDocument;

namespace SportsGurukul.Application.Features.DocumentManagement.Validators;

public class DeleteAthleteDocumentValidator : AbstractValidator<DeleteAthleteDocumentCommand>
{
    public DeleteAthleteDocumentValidator()
    {
        RuleFor(x => x.DocumentId)
            .NotEmpty().WithMessage("Document ID is required.");
    }
}
