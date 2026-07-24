using FluentValidation;
using SportsGurukul.Application.Features.CoachManagement.Commands.DeleteCoachDocument;

namespace SportsGurukul.Application.Features.CoachManagement.Validators;

public class DeleteCoachDocumentValidator : AbstractValidator<DeleteCoachDocumentCommand>
{
    public DeleteCoachDocumentValidator()
    {
        RuleFor(x => x.DocumentId)
            .NotEmpty().WithMessage("Document ID is required.");
    }
}
