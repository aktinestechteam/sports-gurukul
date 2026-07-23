using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.UserManagement.Commands.UploadProfilePhoto;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.UserManagement;

public class UploadProfilePhotoCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUserProfileRepository> _userProfileRepositoryMock;
    private readonly Mock<IFileRepository> _fileRepositoryMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<UploadProfilePhotoCommandHandler>> _loggerMock;
    private readonly UploadProfilePhotoCommandHandler _handler;

    private readonly Guid _userId = Guid.NewGuid();

    public UploadProfilePhotoCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userProfileRepositoryMock = new Mock<IUserProfileRepository>();
        _fileRepositoryMock = new Mock<IFileRepository>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<UploadProfilePhotoCommandHandler>>();
        _handler = new UploadProfilePhotoCommandHandler(
            _userRepositoryMock.Object,
            _userProfileRepositoryMock.Object,
            _fileRepositoryMock.Object,
            _fileStorageServiceMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserNotFound()
    {
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _handler.Handle(CreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("User not found.");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProfileNotFound()
    {
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTestUser());
        _userProfileRepositoryMock
            .Setup(r => r.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);

        var result = await _handler.Handle(CreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Profile not found. Please create a profile first.");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProfileIsDeleted()
    {
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTestUser());
        _userProfileRepositoryMock
            .Setup(r => r.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserProfile { IsDeleted = true });

        var result = await _handler.Handle(CreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Profile not found. Please create a profile first.");
    }

    [Fact]
    public async Task Handle_Should_UploadPhoto_When_FirstUpload()
    {
        var user = CreateTestUser();
        var profile = CreateTestProfile();

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _fileRepositoryMock
            .Setup(r => r.GetByUserIdAndTypeAsync(_userId, FileType.ProfilePhoto, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserFile?)null);
        _fileStorageServiceMock
            .Setup(s => s.UploadAsync(It.IsAny<Stream>(), "photo.jpg", "image/jpeg", FileCategory.Image, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FileStorageResult
            {
                StoredFileName = "stored-photo.jpg",
                StoragePath = "/uploads/stored-photo.jpg",
                PublicUrl = "https://cdn.example.com/stored-photo.jpg",
                FileSize = 1024
            });
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(CreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Url.Should().Be("https://cdn.example.com/stored-photo.jpg");
        result.Value.FileName.Should().Be("photo.jpg");
        result.Value.FileSize.Should().Be(1024);
        result.Value.ContentType.Should().Be("image/jpeg");
    }

    [Fact]
    public async Task Handle_Should_DeleteOldFile_When_Reuploading()
    {
        var user = CreateTestUser();
        var profile = CreateTestProfile();
        var existingFile = new UserFile
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            FileType = FileType.ProfilePhoto,
            StoragePath = "/uploads/old-photo.jpg"
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _fileRepositoryMock
            .Setup(r => r.GetByUserIdAndTypeAsync(_userId, FileType.ProfilePhoto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingFile);
        _fileStorageServiceMock
            .Setup(s => s.DeleteAsync("/uploads/old-photo.jpg", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _fileStorageServiceMock
            .Setup(s => s.UploadAsync(It.IsAny<Stream>(), "new-photo.png", "image/png", FileCategory.Image, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FileStorageResult
            {
                StoredFileName = "stored-new.png",
                StoragePath = "/uploads/stored-new.png",
                PublicUrl = "https://cdn.example.com/stored-new.png",
                FileSize = 2048
            });
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new UploadProfilePhotoCommand
        {
            UserId = _userId,
            FileName = "new-photo.png",
            ContentType = "image/png",
            FileContent = new byte[] { 1, 2, 3 }
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _fileStorageServiceMock.Verify(s => s.DeleteAsync("/uploads/old-photo.jpg", It.IsAny<CancellationToken>()), Times.Once);
        _fileRepositoryMock.Verify(r => r.Remove(existingFile), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_UpdateProfileAndUserImageUrl_When_PhotoUploaded()
    {
        var user = CreateTestUser();
        var profile = CreateTestProfile();

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _fileRepositoryMock
            .Setup(r => r.GetByUserIdAndTypeAsync(_userId, FileType.ProfilePhoto, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserFile?)null);
        _fileStorageServiceMock
            .Setup(s => s.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), FileCategory.Image, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FileStorageResult
            {
                StoredFileName = "stored.jpg",
                StoragePath = "/uploads/stored.jpg",
                PublicUrl = "https://cdn.example.com/stored.jpg",
                FileSize = 512
            });
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _handler.Handle(CreateCommand(), CancellationToken.None);

        profile.ProfileImageUrl.Should().Be("https://cdn.example.com/stored.jpg");
        user.ProfileImageUrl.Should().Be("https://cdn.example.com/stored.jpg");
    }

    [Fact]
    public async Task Handle_Should_UseStoragePath_When_PublicUrlIsNull()
    {
        var user = CreateTestUser();
        var profile = CreateTestProfile();

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _fileRepositoryMock
            .Setup(r => r.GetByUserIdAndTypeAsync(_userId, FileType.ProfilePhoto, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserFile?)null);
        _fileStorageServiceMock
            .Setup(s => s.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), FileCategory.Image, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FileStorageResult
            {
                StoredFileName = "stored.jpg",
                StoragePath = "/uploads/stored.jpg",
                PublicUrl = null,
                FileSize = 512
            });
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(CreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Url.Should().Be("/uploads/stored.jpg");
    }

    [Fact]
    public async Task Handle_Should_CreateUserFile_When_UploadSucceeds()
    {
        var user = CreateTestUser();
        var profile = CreateTestProfile();

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _fileRepositoryMock
            .Setup(r => r.GetByUserIdAndTypeAsync(_userId, FileType.ProfilePhoto, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserFile?)null);
        _fileStorageServiceMock
            .Setup(s => s.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), FileCategory.Image, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FileStorageResult
            {
                StoredFileName = "stored.jpg",
                StoragePath = "/uploads/stored.jpg",
                PublicUrl = "https://cdn.example.com/stored.jpg",
                FileSize = 512
            });
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _handler.Handle(CreateCommand(), CancellationToken.None);

        _fileRepositoryMock.Verify(r => r.AddAsync(
            It.Is<UserFile>(f =>
                f.UserId == _userId &&
                f.OriginalFileName == "photo.jpg" &&
                f.ContentType == "image/jpeg" &&
                f.FileType == FileType.ProfilePhoto &&
                f.FileCategory == FileCategory.Image),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    private UploadProfilePhotoCommand CreateCommand() => new()
    {
        UserId = _userId,
        FileName = "photo.jpg",
        ContentType = "image/jpeg",
        FileContent = new byte[] { 1, 2, 3, 4, 5 }
    };

    private User CreateTestUser() => new()
    {
        Id = _userId,
        Email = "test@example.com",
        FullName = "Test User",
        Status = UserStatus.Active,
        UserRoles = new List<UserRole>()
    };

    private UserProfile CreateTestProfile() => new()
    {
        Id = Guid.NewGuid(),
        UserId = _userId,
        IsDeleted = false,
        Addresses = new List<Address>(),
        ContactInformation = null,
        UserPreference = null
    };
}
