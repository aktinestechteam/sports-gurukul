using FluentValidation;
using SportsGurukul.Application.Features.CoachManagement.Commands.UploadCoachDocument;

namespace SportsGurukul.Application.Features.CoachManagement.Validators;

public class UploadCoachDocumentValidator : AbstractValidator<UploadCoachDocumentCommand>
{
    private static readonly string[] AllowedExtensions = [".pdf", ".jpg", ".jpeg", ".png", ".webp"];
    private static readonly string[] BlockedExtensions = [".exe", ".bat", ".cmd", ".com", ".msi", ".scr", ".pif", ".js", ".vbs", ".ps1", ".sh", ".bat"];
    private const long MaxFileSize = 20 * 1024 * 1024; // 20 MB

    public UploadCoachDocumentValidator()
    {
        RuleFor(x => x.CoachId)
            .NotEmpty().WithMessage("Coach ID is required.");

        RuleFor(x => x.File)
            .NotNull().WithMessage("File is required.");

        RuleFor(x => x.File.FileName)
            .NotEmpty().WithMessage("File name is required.")
            .MaximumLength(255).WithMessage("File name must not exceed 255 characters.")
            .When(x => x.File is not null);

        RuleFor(x => x.File.Length)
            .LessThanOrEqualTo(MaxFileSize).WithMessage($"File size must not exceed {MaxFileSize / (1024 * 1024)} MB.")
            .When(x => x.File is not null);

        RuleFor(x => Path.GetExtension(x.File.FileName).ToLowerInvariant())
            .Must(ext => AllowedExtensions.Contains(ext))
            .WithMessage($"File type is not allowed. Allowed types: {string.Join(", ", AllowedExtensions)}.")
            .When(x => x.File is not null && !string.IsNullOrEmpty(x.File.FileName));

        RuleFor(x => Path.GetExtension(x.File.FileName).ToLowerInvariant())
            .Must(ext => !BlockedExtensions.Contains(ext))
            .WithMessage("Executable and script files are not allowed.")
            .When(x => x.File is not null && !string.IsNullOrEmpty(x.File.FileName));

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Invalid document category.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Remarks)
            .MaximumLength(500).WithMessage("Remarks must not exceed 500 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Remarks));

        RuleFor(x => x.ExpiryDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Expiry date must be in the future.")
            .When(x => x.ExpiryDate.HasValue);
    }
}
