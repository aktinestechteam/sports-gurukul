using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.Authentication.Commands.ForgotPassword;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.UnitTests;

public class ForgotPasswordCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordResetTokenRepository> _tokenRepositoryMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<ForgotPasswordCommandHandler>> _loggerMock;
    private readonly ForgotPasswordCommandHandler _handler;

    public ForgotPasswordCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _tokenRepositoryMock = new Mock<IPasswordResetTokenRepository>();
        _emailServiceMock = new Mock<IEmailService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<ForgotPasswordCommandHandler>>();
        _handler = new ForgotPasswordCommandHandler(
            _userRepositoryMock.Object,
            _tokenRepositoryMock.Object,
            _emailServiceMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsSuccessWithoutSendingEmail()
    {
        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync("unknown@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _handler.Handle(
            new ForgotPasswordCommand { Email = "unknown@example.com" },
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        _emailServiceMock.Verify(s => s.SendAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_UserNotFound_DoesNotCreateToken()
    {
        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync("unknown@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        await _handler.Handle(
            new ForgotPasswordCommand { Email = "unknown@example.com" },
            CancellationToken.None);

        _tokenRepositoryMock.Verify(r => r.AddAsync(
            It.IsAny<PasswordResetToken>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ValidUser_CreatesTokenAndSendsEmail()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FullName = "Test User",
            IsEmailVerified = true
        };
        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync("test@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _tokenRepositoryMock
            .Setup(r => r.InvalidateAllUserTokensAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _tokenRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<PasswordResetToken>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PasswordResetToken());
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new ForgotPasswordCommand { Email = "test@example.com" },
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        _emailServiceMock.Verify(s => s.SendAsync(
            "test@example.com",
            It.IsAny<string>(),
            It.Is<string>(h => h.Contains("Test User")),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidUser_InvalidatesExistingTokens()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FullName = "Test User"
        };
        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync("test@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _tokenRepositoryMock
            .Setup(r => r.InvalidateAllUserTokensAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);
        _tokenRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<PasswordResetToken>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PasswordResetToken());
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _handler.Handle(
            new ForgotPasswordCommand { Email = "test@example.com" },
            CancellationToken.None);

        _tokenRepositoryMock.Verify(r => r.InvalidateAllUserTokensAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidUser_CreatesTokenWithCorrectExpiry()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FullName = "Test User"
        };
        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync("test@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _tokenRepositoryMock
            .Setup(r => r.InvalidateAllUserTokensAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        PasswordResetToken? capturedToken = null;
        _tokenRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<PasswordResetToken>(), It.IsAny<CancellationToken>()))
            .Callback<PasswordResetToken, CancellationToken>((t, _) => capturedToken = t)
            .ReturnsAsync(new PasswordResetToken());
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _handler.Handle(
            new ForgotPasswordCommand { Email = "test@example.com" },
            CancellationToken.None);

        Assert.NotNull(capturedToken);
        Assert.Equal(user.Id, capturedToken!.UserId);
        Assert.False(string.IsNullOrEmpty(capturedToken.Token));
        Assert.True(capturedToken.ExpiresAt > DateTime.UtcNow);
        Assert.Null(capturedToken.UsedAt);
    }
}
