using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.Authentication.Commands.ResetPassword;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests;

public class ResetPasswordCommandHandlerTests
{
    private readonly Mock<IPasswordResetTokenRepository> _tokenRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<ResetPasswordCommandHandler>> _loggerMock;
    private readonly ResetPasswordCommandHandler _handler;

    public ResetPasswordCommandHandlerTests()
    {
        _tokenRepositoryMock = new Mock<IPasswordResetTokenRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<ResetPasswordCommandHandler>>();
        _handler = new ResetPasswordCommandHandler(
            _tokenRepositoryMock.Object,
            _userRepositoryMock.Object,
            _refreshTokenRepositoryMock.Object,
            _passwordHasherMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_InvalidToken_ReturnsFailure()
    {
        _tokenRepositoryMock
            .Setup(r => r.GetByTokenAsync("bad-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync((PasswordResetToken?)null);

        var result = await _handler.Handle(
            new ResetPasswordCommand
            {
                Token = "bad-token",
                NewPassword = "NewP@ss1",
                ConfirmNewPassword = "NewP@ss1"
            },
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid reset token.", result.Error);
    }

    [Fact]
    public async Task Handle_TokenAlreadyUsed_ReturnsFailure()
    {
        var tokenEntity = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            Token = "used-token",
            UserId = Guid.NewGuid(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            UsedAt = DateTime.UtcNow.AddMinutes(-5)
        };
        _tokenRepositoryMock
            .Setup(r => r.GetByTokenAsync("used-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenEntity);

        var result = await _handler.Handle(
            new ResetPasswordCommand
            {
                Token = "used-token",
                NewPassword = "NewP@ss1",
                ConfirmNewPassword = "NewP@ss1"
            },
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Reset token has already been used.", result.Error);
    }

    [Fact]
    public async Task Handle_TokenExpired_ReturnsFailure()
    {
        var tokenEntity = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            Token = "expired-token",
            UserId = Guid.NewGuid(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(-1),
            UsedAt = null
        };
        _tokenRepositoryMock
            .Setup(r => r.GetByTokenAsync("expired-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenEntity);

        var result = await _handler.Handle(
            new ResetPasswordCommand
            {
                Token = "expired-token",
                NewPassword = "NewP@ss1",
                ConfirmNewPassword = "NewP@ss1"
            },
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("expired", result.Error!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsFailure()
    {
        var tokenEntity = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            Token = "valid-token",
            UserId = Guid.NewGuid(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            UsedAt = null
        };
        _tokenRepositoryMock
            .Setup(r => r.GetByTokenAsync("valid-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenEntity);
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(tokenEntity.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _handler.Handle(
            new ResetPasswordCommand
            {
                Token = "valid-token",
                NewPassword = "NewP@ss1",
                ConfirmNewPassword = "NewP@ss1"
            },
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("User not found.", result.Error);
    }

    [Fact]
    public async Task Handle_PasswordMismatch_ReturnsFailure()
    {
        var result = await _handler.Handle(
            new ResetPasswordCommand
            {
                Token = "some-token",
                NewPassword = "NewP@ss1",
                ConfirmNewPassword = "DifferentP@ss1"
            },
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Passwords do not match.", result.Error);
    }

    [Fact]
    public async Task Handle_NewPasswordMatchesCurrent_ReturnsFailure()
    {
        var tokenEntity = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            Token = "valid-token",
            UserId = Guid.NewGuid(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            UsedAt = null
        };
        var user = new User
        {
            Id = tokenEntity.UserId,
            Email = "test@example.com",
            FullName = "Test User",
            PasswordHash = "current-hash"
        };
        _tokenRepositoryMock
            .Setup(r => r.GetByTokenAsync("valid-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenEntity);
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock
            .Setup(h => h.VerifyPassword("current-hash", "NewP@ss1"))
            .Returns(true);

        var result = await _handler.Handle(
            new ResetPasswordCommand
            {
                Token = "valid-token",
                NewPassword = "NewP@ss1",
                ConfirmNewPassword = "NewP@ss1"
            },
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("same as", result.Error!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_ValidReset_UpdatesPasswordAndRevokesTokens()
    {
        var tokenEntity = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            Token = "valid-token",
            UserId = Guid.NewGuid(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            UsedAt = null
        };
        var user = new User
        {
            Id = tokenEntity.UserId,
            Email = "test@example.com",
            FullName = "Test User",
            PasswordHash = "old-hash",
            Status = UserStatus.Active,
            FailedLoginAttempts = 3
        };
        _tokenRepositoryMock
            .Setup(r => r.GetByTokenAsync("valid-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenEntity);
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock
            .Setup(h => h.VerifyPassword("old-hash", "NewP@ss1"))
            .Returns(false);
        _passwordHasherMock
            .Setup(h => h.HashPassword("NewP@ss1"))
            .Returns("new-hash");
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new ResetPasswordCommand
            {
                Token = "valid-token",
                NewPassword = "NewP@ss1",
                ConfirmNewPassword = "NewP@ss1"
            },
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("new-hash", user.PasswordHash);
        Assert.Equal(0, user.FailedLoginAttempts);
        Assert.Null(user.LockoutEndAt);
        Assert.NotNull(tokenEntity.UsedAt);
        _refreshTokenRepositoryMock.Verify(r =>
            r.RevokeAllUserTokensAsync(user.Id, "Password reset", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_LockedAccount_UnlocksAfterReset()
    {
        var tokenEntity = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            Token = "valid-token",
            UserId = Guid.NewGuid(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            UsedAt = null
        };
        var user = new User
        {
            Id = tokenEntity.UserId,
            Email = "locked@example.com",
            FullName = "Locked User",
            PasswordHash = "old-hash",
            Status = UserStatus.Locked,
            LockoutEndAt = DateTime.UtcNow.AddMinutes(10)
        };
        _tokenRepositoryMock
            .Setup(r => r.GetByTokenAsync("valid-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenEntity);
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock
            .Setup(h => h.VerifyPassword("old-hash", "NewP@ss1"))
            .Returns(false);
        _passwordHasherMock
            .Setup(h => h.HashPassword("NewP@ss1"))
            .Returns("new-hash");
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new ResetPasswordCommand
            {
                Token = "valid-token",
                NewPassword = "NewP@ss1",
                ConfirmNewPassword = "NewP@ss1"
            },
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(UserStatus.Active, user.Status);
        Assert.Null(user.LockoutEndAt);
    }
}
