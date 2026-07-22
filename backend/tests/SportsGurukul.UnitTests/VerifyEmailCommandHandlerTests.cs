using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.Authentication.Commands.VerifyEmail;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.UnitTests;

public class VerifyEmailCommandHandlerTests
{
    private readonly Mock<IEmailVerificationTokenRepository> _tokenRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<VerifyEmailCommandHandler>> _loggerMock;
    private readonly VerifyEmailCommandHandler _handler;

    public VerifyEmailCommandHandlerTests()
    {
        _tokenRepositoryMock = new Mock<IEmailVerificationTokenRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<VerifyEmailCommandHandler>>();
        _handler = new VerifyEmailCommandHandler(
            _tokenRepositoryMock.Object,
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_InvalidToken_ReturnsFailure()
    {
        _tokenRepositoryMock
            .Setup(r => r.GetByTokenAsync("bad-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync((EmailVerificationToken?)null);

        var result = await _handler.Handle(
            new VerifyEmailCommand { Token = "bad-token" },
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid verification token.", result.Error);
    }

    [Fact]
    public async Task Handle_TokenAlreadyUsed_ReturnsFailure()
    {
        var tokenEntity = new EmailVerificationToken
        {
            Id = Guid.NewGuid(),
            Token = "used-token",
            UserId = Guid.NewGuid(),
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            UsedAt = DateTime.UtcNow.AddHours(-1)
        };
        _tokenRepositoryMock
            .Setup(r => r.GetByTokenAsync("used-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenEntity);

        var result = await _handler.Handle(
            new VerifyEmailCommand { Token = "used-token" },
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Verification token has already been used.", result.Error);
    }

    [Fact]
    public async Task Handle_TokenExpired_ReturnsFailure()
    {
        var tokenEntity = new EmailVerificationToken
        {
            Id = Guid.NewGuid(),
            Token = "expired-token",
            UserId = Guid.NewGuid(),
            ExpiresAt = DateTime.UtcNow.AddHours(-1),
            UsedAt = null
        };
        _tokenRepositoryMock
            .Setup(r => r.GetByTokenAsync("expired-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenEntity);

        var result = await _handler.Handle(
            new VerifyEmailCommand { Token = "expired-token" },
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("expired", result.Error!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsFailure()
    {
        var tokenEntity = new EmailVerificationToken
        {
            Id = Guid.NewGuid(),
            Token = "valid-token",
            UserId = Guid.NewGuid(),
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            UsedAt = null
        };
        _tokenRepositoryMock
            .Setup(r => r.GetByTokenAsync("valid-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenEntity);
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(tokenEntity.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _handler.Handle(
            new VerifyEmailCommand { Token = "valid-token" },
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("User not found.", result.Error);
    }

    [Fact]
    public async Task Handle_ValidToken_MarksEmailVerifiedAndReturnsSuccess()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FullName = "Test User",
            IsEmailVerified = false
        };
        var tokenEntity = new EmailVerificationToken
        {
            Id = Guid.NewGuid(),
            Token = "valid-token",
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            UsedAt = null
        };
        _tokenRepositoryMock
            .Setup(r => r.GetByTokenAsync("valid-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenEntity);
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new VerifyEmailCommand { Token = "valid-token" },
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(user.IsEmailVerified);
        Assert.NotNull(tokenEntity.UsedAt);
    }

    [Fact]
    public async Task Handle_AlreadyVerified_ReturnsSuccessWithoutChanges()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "verified@example.com",
            FullName = "Already Verified",
            IsEmailVerified = true
        };
        var tokenEntity = new EmailVerificationToken
        {
            Id = Guid.NewGuid(),
            Token = "token-for-verified",
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            UsedAt = null
        };
        _tokenRepositoryMock
            .Setup(r => r.GetByTokenAsync("token-for-verified", It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenEntity);
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _handler.Handle(
            new VerifyEmailCommand { Token = "token-for-verified" },
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
