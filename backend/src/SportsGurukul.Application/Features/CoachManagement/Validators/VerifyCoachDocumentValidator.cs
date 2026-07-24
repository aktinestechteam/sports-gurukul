using FluentValidation;
using SportsGurukul.Application.Features.CoachManagement.Commands.VerifyCoachDocument;

namespace SportsGurukul.Application.Features.CoachManagement.Validators;

public class VerifyCoachDocumentValidator : AbstractValidator<VerifyCoachDocumentCommand>
{
    public VerifyCoachDocumentValidator()
    {
        RuleFor(x => x.DocumentId)
            .NotEmpty().WithMessage("Document ID is required.");

        RuleFor(x => x.Comments)
            .MaximumLength(1000).WithMessage("Comments must not exceed 1000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Comments));
    }
}
