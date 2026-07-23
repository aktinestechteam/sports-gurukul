using FluentValidation;
using SportsGurukul.Application.Features.DocumentManagement.Commands.UploadAthleteDocument;

namespace SportsGurukul.Application.Features.DocumentManagement.Validators;

public class UploadAthleteDocumentValidator : AbstractValidator<UploadAthleteDocumentCommand>
{
    private static readonly string[] AllowedExtensions =
    [
        ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx",
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp",
        ".txt", ".csv", ".rtf", ".odt"
    ];

    private const long MaxFileSize = 10 * 1024 * 1024;

    public UploadAthleteDocumentValidator()
    {
        RuleFor(x => x.AthleteId)
            .NotEmpty().WithMessage("Athlete ID is required.");

        RuleFor(x => x.File)
            .NotEmpty().WithMessage("File is required.");

        RuleFor(x => x.File.FileName)
            .NotEmpty().WithMessage("File name is required.")
            .MaximumLength(255).WithMessage("File name must not exceed 255 characters.")
            .When(x => x.File is not null);

        RuleFor(x => x.File.Length)
            .LessThanOrEqualTo(MaxFileSize).WithMessage($"File size must not exceed {MaxFileSize / (1024 * 1024)} MB.")
            .When(x => x.File is not null);

        RuleFor(x => Path.GetExtension(x.File.FileName).ToLowerInvariant())
            .Must(ext => AllowedExtensions.Contains(ext))
            .WithMessage("File type is not allowed.")
            .When(x => x.File is not null && !string.IsNullOrEmpty(x.File.FileName));

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Invalid document category.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.ExpiryDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Expiry date must be in the future.")
            .When(x => x.ExpiryDate.HasValue);
    }
}
