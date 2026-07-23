using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.UserManagement.Commands.UploadProfilePhoto;
using SportsGurukul.Application.Features.UserManagement.Validators;

namespace SportsGurukul.UnitTests.UserManagement;

public class UploadProfilePhotoValidatorTests
{
    private readonly UploadProfilePhotoValidator _validator = new();

    [Fact]
    public async Task Validate_Should_Pass_When_ValidJpegImage()
    {
        var command = new UploadProfilePhotoCommand
        {
            UserId = Guid.NewGuid(),
            FileName = "photo.jpg",
            ContentType = "image/jpeg",
            FileContent = new byte[1024]
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_Should_Pass_When_ValidPngImage()
    {
        var command = new UploadProfilePhotoCommand
        {
            UserId = Guid.NewGuid(),
            FileName = "photo.png",
            ContentType = "image/png",
            FileContent = new byte[1024]
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_Should_Pass_When_ValidWebpImage()
    {
        var command = new UploadProfilePhotoCommand
        {
            UserId = Guid.NewGuid(),
            FileName = "photo.webp",
            ContentType = "image/webp",
            FileContent = new byte[1024]
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_Should_Fail_When_UserIdIsEmpty()
    {
        var command = new UploadProfilePhotoCommand
        {
            UserId = Guid.Empty,
            FileName = "photo.jpg",
            ContentType = "image/jpeg",
            FileContent = new byte[1024]
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID is required.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_FileNameIsEmpty()
    {
        var command = new UploadProfilePhotoCommand
        {
            UserId = Guid.NewGuid(),
            FileName = "",
            ContentType = "image/jpeg",
            FileContent = new byte[1024]
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.FileName)
            .WithErrorMessage("File name is required.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_FileNameExceeds255Characters()
    {
        var command = new UploadProfilePhotoCommand
        {
            UserId = Guid.NewGuid(),
            FileName = new string('a', 256) + ".jpg",
            ContentType = "image/jpeg",
            FileContent = new byte[1024]
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.FileName)
            .WithErrorMessage("File name must not exceed 255 characters.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_ContentTypeIsEmpty()
    {
        var command = new UploadProfilePhotoCommand
        {
            UserId = Guid.NewGuid(),
            FileName = "photo.jpg",
            ContentType = "",
            FileContent = new byte[1024]
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.ContentType)
            .WithErrorMessage("Content type is required.");
    }

    [Theory]
    [InlineData("image/gif")]
    [InlineData("image/bmp")]
    [InlineData("image/tiff")]
    [InlineData("application/pdf")]
    [InlineData("video/mp4")]
    public async Task Validate_Should_Fail_When_ContentTypeIsNotAllowed(string contentType)
    {
        var command = new UploadProfilePhotoCommand
        {
            UserId = Guid.NewGuid(),
            FileName = "file.txt",
            ContentType = contentType,
            FileContent = new byte[1024]
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.ContentType)
            .WithErrorMessage("Only JPEG, PNG, and WebP images are allowed.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_FileContentIsEmpty()
    {
        var command = new UploadProfilePhotoCommand
        {
            UserId = Guid.NewGuid(),
            FileName = "photo.jpg",
            ContentType = "image/jpeg",
            FileContent = Array.Empty<byte>()
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.FileContent)
            .WithErrorMessage("File content is required.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_FileSizeExceeds5MB()
    {
        var command = new UploadProfilePhotoCommand
        {
            UserId = Guid.NewGuid(),
            FileName = "photo.jpg",
            ContentType = "image/jpeg",
            FileContent = new byte[5 * 1024 * 1024 + 1]
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.FileContent)
            .WithErrorMessage("File size must not exceed 5 MB.");
    }

    [Fact]
    public async Task Validate_Should_Pass_When_FileSizeIsExactly5MB()
    {
        var command = new UploadProfilePhotoCommand
        {
            UserId = Guid.NewGuid(),
            FileName = "photo.jpg",
            ContentType = "image/jpeg",
            FileContent = new byte[5 * 1024 * 1024]
        };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(x => x.FileContent);
    }
}
