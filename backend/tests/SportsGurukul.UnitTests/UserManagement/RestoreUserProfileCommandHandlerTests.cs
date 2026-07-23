using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.UserManagement.Commands.RestoreUserProfile;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.UnitTests.UserManagement;

public class RestoreUserProfileCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUserProfileRepository> _userProfileRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<RestoreUserProfileCommandHandler>> _loggerMock;
    private readonly RestoreUserProfileCommandHandler _handler;

    private readonly Guid _userId = Guid.NewGuid();

    public RestoreUserProfileCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userProfileRepositoryMock = new Mock<IUserProfileRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<RestoreUserProfileCommandHandler>>();
        _handler = new RestoreUserProfileCommandHandler(
            _userRepositoryMock.Object,
            _userProfileRepositoryMock.Object,
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
            new RestoreUserProfileCommand { UserId = _userId },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("User not found.");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_NoDeletedProfileFound()
    {
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = _userId });
        _userProfileRepositoryMock
            .Setup(r => r.GetDeletedByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);

        var result = await _handler.Handle(
            new RestoreUserProfileCommand { UserId = _userId },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("No deleted profile found for this user.");
    }

    [Fact]
    public async Task Handle_Should_RestoreProfile_When_DeletedProfileExists()
    {
        var deletedProfile = new UserProfile
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            IsDeleted = true
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = _userId });
        _userProfileRepositoryMock
            .Setup(r => r.GetDeletedByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deletedProfile);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new RestoreUserProfileCommand { UserId = _userId },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        deletedProfile.IsDeleted.Should().BeFalse();
        deletedProfile.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Should_CallUpdate_When_ProfileRestored()
    {
        var deletedProfile = new UserProfile
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            IsDeleted = true
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = _userId });
        _userProfileRepositoryMock
            .Setup(r => r.GetDeletedByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deletedProfile);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _handler.Handle(
            new RestoreUserProfileCommand { UserId = _userId },
            CancellationToken.None);

        _userProfileRepositoryMock.Verify(r => r.Update(deletedProfile), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_OnlyActiveProfilesExist()
    {
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = _userId });
        _userProfileRepositoryMock
            .Setup(r => r.GetDeletedByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);

        var result = await _handler.Handle(
            new RestoreUserProfileCommand { UserId = _userId },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("No deleted profile found for this user.");
    }

    [Fact]
    public async Task Handle_Should_CallSaveChanges_When_ProfileRestored()
    {
        var deletedProfile = new UserProfile
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            IsDeleted = true
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = _userId });
        _userProfileRepositoryMock
            .Setup(r => r.GetDeletedByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deletedProfile);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _handler.Handle(
            new RestoreUserProfileCommand { UserId = _userId },
            CancellationToken.None);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
