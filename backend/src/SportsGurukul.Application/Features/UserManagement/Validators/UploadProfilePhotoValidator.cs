using FluentValidation;
using SportsGurukul.Application.Features.UserManagement.Commands.UploadProfilePhoto;

namespace SportsGurukul.Application.Features.UserManagement.Validators;

public class UploadProfilePhotoValidator : AbstractValidator<UploadProfilePhotoCommand>
{
    private static readonly string[] AllowedContentTypes =
    [
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/webp"
    ];

    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    public UploadProfilePhotoValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("File name is required.")
            .MaximumLength(255).WithMessage("File name must not exceed 255 characters.");

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("Content type is required.")
            .Must(ct => AllowedContentTypes.Contains(ct.ToLowerInvariant()))
                .WithMessage("Only JPEG, PNG, and WebP images are allowed.");

        RuleFor(x => x.FileContent)
            .NotEmpty().WithMessage("File content is required.")
            .Must(content => content.Length <= MaxFileSizeBytes)
                .WithMessage("File size must not exceed 5 MB.");
    }
}
