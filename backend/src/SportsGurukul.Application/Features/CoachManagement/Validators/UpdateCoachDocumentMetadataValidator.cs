using FluentValidation;
using SportsGurukul.Application.Features.CoachManagement.Commands.UpdateCoachDocumentMetadata;

namespace SportsGurukul.Application.Features.CoachManagement.Validators;

public class UpdateCoachDocumentMetadataValidator : AbstractValidator<UpdateCoachDocumentMetadataCommand>
{
    public UpdateCoachDocumentMetadataValidator()
    {
        RuleFor(x => x.DocumentId)
            .NotEmpty().WithMessage("Document ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Invalid document category.");

        RuleFor(x => x.Remarks)
            .MaximumLength(500).WithMessage("Remarks must not exceed 500 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Remarks));

        RuleFor(x => x.ExpiryDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Expiry date must be in the future.")
            .When(x => x.ExpiryDate.HasValue);
    }
}
