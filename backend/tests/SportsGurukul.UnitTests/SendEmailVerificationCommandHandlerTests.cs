using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.Authentication.Commands.SendEmailVerification;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.UnitTests;

public class SendEmailVerificationCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IEmailVerificationTokenRepository> _tokenRepositoryMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<SendEmailVerificationCommandHandler>> _loggerMock;
    private readonly SendEmailVerificationCommandHandler _handler;

    public SendEmailVerificationCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _tokenRepositoryMock = new Mock<IEmailVerificationTokenRepository>();
        _emailServiceMock = new Mock<IEmailService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<SendEmailVerificationCommandHandler>>();
        _handler = new SendEmailVerificationCommandHandler(
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
            new SendEmailVerificationCommand { Email = "unknown@example.com" },
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        _emailServiceMock.Verify(s => s.SendAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_EmailAlreadyVerified_ReturnsSuccessWithoutSendingEmail()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "verified@example.com",
            FullName = "Verified User",
            IsEmailVerified = true
        };
        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync("verified@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _handler.Handle(
            new SendEmailVerificationCommand { Email = "verified@example.com" },
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        _emailServiceMock.Verify(s => s.SendAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ValidUnverifiedUser_SendsEmailAndReturnsSuccess()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "unverified@example.com",
            FullName = "Test User",
            IsEmailVerified = false
        };
        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync("unverified@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _tokenRepositoryMock
            .Setup(r => r.InvalidateAllUserTokensAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _tokenRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<EmailVerificationToken>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EmailVerificationToken());
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new SendEmailVerificationCommand { Email = "unverified@example.com" },
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        _emailServiceMock.Verify(s => s.SendAsync(
            "unverified@example.com",
            It.IsAny<string>(),
            It.Is<string>(h => h.Contains("Test User")),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidUnverifiedUser_InvalidatesExistingTokens()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FullName = "Test User",
            IsEmailVerified = false
        };
        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync("test@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _tokenRepositoryMock
            .Setup(r => r.InvalidateAllUserTokensAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);
        _tokenRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<EmailVerificationToken>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EmailVerificationToken());
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _handler.Handle(
            new SendEmailVerificationCommand { Email = "test@example.com" },
            CancellationToken.None);

        _tokenRepositoryMock.Verify(r => r.InvalidateAllUserTokensAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidUnverifiedUser_CreatesTokenWithCorrectExpiry()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FullName = "Test User",
            IsEmailVerified = false
        };
        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync("test@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _tokenRepositoryMock
            .Setup(r => r.InvalidateAllUserTokensAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        EmailVerificationToken? capturedToken = null;
        _tokenRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<EmailVerificationToken>(), It.IsAny<CancellationToken>()))
            .Callback<EmailVerificationToken, CancellationToken>((t, _) => capturedToken = t)
            .ReturnsAsync(new EmailVerificationToken());
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _handler.Handle(
            new SendEmailVerificationCommand { Email = "test@example.com" },
            CancellationToken.None);

        Assert.NotNull(capturedToken);
        Assert.Equal(user.Id, capturedToken!.UserId);
        Assert.False(string.IsNullOrEmpty(capturedToken.Token));
        Assert.True(capturedToken.ExpiresAt > DateTime.UtcNow);
        Assert.Null(capturedToken.UsedAt);
    }
}
