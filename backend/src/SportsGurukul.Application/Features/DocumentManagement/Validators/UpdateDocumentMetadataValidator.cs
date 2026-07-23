using FluentValidation;
using SportsGurukul.Application.Features.DocumentManagement.Commands.UpdateDocumentMetadata;

namespace SportsGurukul.Application.Features.DocumentManagement.Validators;

public class UpdateDocumentMetadataValidator : AbstractValidator<UpdateDocumentMetadataCommand>
{
    public UpdateDocumentMetadataValidator()
    {
        RuleFor(x => x.DocumentId)
            .NotEmpty().WithMessage("Document ID is required.");

        RuleFor(x => x.Title)
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Title));

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Invalid document category.")
            .When(x => x.Category.HasValue);

        RuleFor(x => x.ExpiryDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Expiry date must be in the future.")
            .When(x => x.ExpiryDate.HasValue);
    }
}
