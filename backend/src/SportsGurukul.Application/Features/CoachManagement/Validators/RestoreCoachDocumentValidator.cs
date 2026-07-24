using FluentValidation;
using SportsGurukul.Application.Features.CoachManagement.Commands.RestoreCoachDocument;

namespace SportsGurukul.Application.Features.CoachManagement.Validators;

public class RestoreCoachDocumentValidator : AbstractValidator<RestoreCoachDocumentCommand>
{
    public RestoreCoachDocumentValidator()
    {
        RuleFor(x => x.DocumentId)
            .NotEmpty().WithMessage("Document ID is required.");
    }
}
