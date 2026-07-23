using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.UserManagement.Commands.DeleteProfilePhoto;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.UserManagement;

public class DeleteProfilePhotoCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUserProfileRepository> _userProfileRepositoryMock;
    private readonly Mock<IFileRepository> _fileRepositoryMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<DeleteProfilePhotoCommandHandler>> _loggerMock;
    private readonly DeleteProfilePhotoCommandHandler _handler;

    private readonly Guid _userId = Guid.NewGuid();

    public DeleteProfilePhotoCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userProfileRepositoryMock = new Mock<IUserProfileRepository>();
        _fileRepositoryMock = new Mock<IFileRepository>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<DeleteProfilePhotoCommandHandler>>();
        _handler = new DeleteProfilePhotoCommandHandler(
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

        var result = await _handler.Handle(
            new DeleteProfilePhotoCommand { UserId = _userId },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("User not found.");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProfileNotFound()
    {
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = _userId });
        _userProfileRepositoryMock
            .Setup(r => r.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);

        var result = await _handler.Handle(
            new DeleteProfilePhotoCommand { UserId = _userId },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Profile not found.");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProfileIsDeleted()
    {
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = _userId });
        _userProfileRepositoryMock
            .Setup(r => r.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserProfile { IsDeleted = true });

        var result = await _handler.Handle(
            new DeleteProfilePhotoCommand { UserId = _userId },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Profile not found.");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_NoPhotoExists()
    {
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = _userId });
        _userProfileRepositoryMock
            .Setup(r => r.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserProfile { IsDeleted = false });
        _fileRepositoryMock
            .Setup(r => r.GetByUserIdAndTypeAsync(_userId, FileType.ProfilePhoto, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserFile?)null);

        var result = await _handler.Handle(
            new DeleteProfilePhotoCommand { UserId = _userId },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("No profile photo to delete.");
    }

    [Fact]
    public async Task Handle_Should_DeletePhoto_When_PhotoExists()
    {
        var user = new User { Id = _userId, ProfileImageUrl = "https://cdn.example.com/photo.jpg" };
        var profile = new UserProfile { Id = Guid.NewGuid(), UserId = _userId, ProfileImageUrl = "https://cdn.example.com/photo.jpg" };
        var file = new UserFile
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            FileType = FileType.ProfilePhoto,
            StoragePath = "/uploads/photo.jpg"
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _fileRepositoryMock
            .Setup(r => r.GetByUserIdAndTypeAsync(_userId, FileType.ProfilePhoto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(file);
        _fileStorageServiceMock
            .Setup(s => s.DeleteAsync("/uploads/photo.jpg", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new DeleteProfilePhotoCommand { UserId = _userId },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _fileStorageServiceMock.Verify(s => s.DeleteAsync("/uploads/photo.jpg", It.IsAny<CancellationToken>()), Times.Once);
        _fileRepositoryMock.Verify(r => r.Remove(file), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ClearImageUrls_When_PhotoDeleted()
    {
        var user = new User { Id = _userId, ProfileImageUrl = "https://cdn.example.com/photo.jpg" };
        var profile = new UserProfile { Id = Guid.NewGuid(), UserId = _userId, ProfileImageUrl = "https://cdn.example.com/photo.jpg" };
        var file = new UserFile
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            FileType = FileType.ProfilePhoto,
            StoragePath = "/uploads/photo.jpg"
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _fileRepositoryMock
            .Setup(r => r.GetByUserIdAndTypeAsync(_userId, FileType.ProfilePhoto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(file);
        _fileStorageServiceMock
            .Setup(s => s.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _handler.Handle(
            new DeleteProfilePhotoCommand { UserId = _userId },
            CancellationToken.None);

        profile.ProfileImageUrl.Should().BeNull();
        user.ProfileImageUrl.Should().BeNull();
    }

    [Fact]
    public async Task Handle_Should_CallSaveChanges_When_PhotoDeleted()
    {
        var user = new User { Id = _userId };
        var profile = new UserProfile { Id = Guid.NewGuid(), UserId = _userId };
        var file = new UserFile
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            FileType = FileType.ProfilePhoto,
            StoragePath = "/uploads/photo.jpg"
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _fileRepositoryMock
            .Setup(r => r.GetByUserIdAndTypeAsync(_userId, FileType.ProfilePhoto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(file);
        _fileStorageServiceMock
            .Setup(s => s.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _handler.Handle(
            new DeleteProfilePhotoCommand { UserId = _userId },
            CancellationToken.None);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
