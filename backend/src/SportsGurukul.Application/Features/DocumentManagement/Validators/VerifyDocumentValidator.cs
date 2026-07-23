using FluentValidation;
using SportsGurukul.Application.Features.DocumentManagement.Commands.VerifyDocument;

namespace SportsGurukul.Application.Features.DocumentManagement.Validators;

public class VerifyDocumentValidator : AbstractValidator<VerifyDocumentCommand>
{
    public VerifyDocumentValidator()
    {
        RuleFor(x => x.DocumentId)
            .NotEmpty().WithMessage("Document ID is required.");
    }
}
