using FluentValidation;
using SportsGurukul.Application.Features.CoachManagement.Commands.RejectCoachDocument;

namespace SportsGurukul.Application.Features.CoachManagement.Validators;

public class RejectCoachDocumentValidator : AbstractValidator<RejectCoachDocumentCommand>
{
    public RejectCoachDocumentValidator()
    {
        RuleFor(x => x.DocumentId)
            .NotEmpty().WithMessage("Document ID is required.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Rejection reason is required.")
            .MaximumLength(1000).WithMessage("Reason must not exceed 1000 characters.");
    }
}
