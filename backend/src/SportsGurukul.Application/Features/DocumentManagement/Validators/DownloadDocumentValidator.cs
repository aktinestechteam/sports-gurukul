using FluentValidation;
using SportsGurukul.Application.Features.DocumentManagement.Queries.DownloadDocument;

namespace SportsGurukul.Application.Features.DocumentManagement.Validators;

public class DownloadDocumentValidator : AbstractValidator<DownloadDocumentQuery>
{
    public DownloadDocumentValidator()
    {
        RuleFor(x => x.DocumentId)
            .NotEmpty().WithMessage("Document ID is required.");
    }
}
