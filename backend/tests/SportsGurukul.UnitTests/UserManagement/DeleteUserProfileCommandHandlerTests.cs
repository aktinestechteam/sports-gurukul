using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.UserManagement.Commands.DeleteUserProfile;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.UserManagement;

public class DeleteUserProfileCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUserProfileRepository> _userProfileRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<DeleteUserProfileCommandHandler>> _loggerMock;
    private readonly DeleteUserProfileCommandHandler _handler;

    private readonly Guid _userId = Guid.NewGuid();

    public DeleteUserProfileCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userProfileRepositoryMock = new Mock<IUserProfileRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<DeleteUserProfileCommandHandler>>();
        _handler = new DeleteUserProfileCommandHandler(
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
            new DeleteUserProfileCommand { UserId = _userId },
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
            new DeleteUserProfileCommand { UserId = _userId },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Profile not found.");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProfileAlreadyDeleted()
    {
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = _userId });
        _userProfileRepositoryMock
            .Setup(r => r.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserProfile { Id = Guid.NewGuid(), UserId = _userId, IsDeleted = true });

        var result = await _handler.Handle(
            new DeleteUserProfileCommand { UserId = _userId },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Profile not found.");
    }

    [Fact]
    public async Task Handle_Should_SoftDeleteProfile_When_ProfileExists()
    {
        var profile = new UserProfile { Id = Guid.NewGuid(), UserId = _userId, IsDeleted = false };
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = _userId });
        _userProfileRepositoryMock
            .Setup(r => r.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new DeleteUserProfileCommand { UserId = _userId },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _userProfileRepositoryMock.Verify(r => r.Remove(profile), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_CallSaveChanges_When_ProfileDeleted()
    {
        var profile = new UserProfile { Id = Guid.NewGuid(), UserId = _userId, IsDeleted = false };
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = _userId });
        _userProfileRepositoryMock
            .Setup(r => r.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _handler.Handle(
            new DeleteUserProfileCommand { UserId = _userId },
            CancellationToken.None);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
