using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.UserManagement.Commands.UpdateUserPreference;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.UserManagement;

public class UpdateUserPreferenceCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUserProfileRepository> _userProfileRepositoryMock;
    private readonly Mock<IRepository<UserPreference>> _preferenceRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<UpdateUserPreferenceCommandHandler>> _loggerMock;
    private readonly UpdateUserPreferenceCommandHandler _handler;

    private readonly Guid _userId = Guid.NewGuid();

    public UpdateUserPreferenceCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userProfileRepositoryMock = new Mock<IUserProfileRepository>();
        _preferenceRepositoryMock = new Mock<IRepository<UserPreference>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<UpdateUserPreferenceCommandHandler>>();
        _handler = new UpdateUserPreferenceCommandHandler(
            _userRepositoryMock.Object,
            _userProfileRepositoryMock.Object,
            _preferenceRepositoryMock.Object,
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
            new UpdateUserPreferenceCommand { UserId = _userId },
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
            new UpdateUserPreferenceCommand { UserId = _userId },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Profile not found. Please create a profile first.");
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
            new UpdateUserPreferenceCommand { UserId = _userId },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Profile not found. Please create a profile first.");
    }

    [Fact]
    public async Task Handle_Should_CreateNewPreference_When_NoExistingPreference()
    {
        var user = new User { Id = _userId };
        var profile = new UserProfile { Id = Guid.NewGuid(), UserId = _userId, UserPreference = null };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new UpdateUserPreferenceCommand
            {
                UserId = _userId,
                Language = "hi",
                Theme = Theme.Dark
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _preferenceRepositoryMock.Verify(r => r.AddAsync(
            It.Is<UserPreference>(p =>
                p.UserProfileId == profile.Id &&
                p.Language == "hi" &&
                p.Theme == Theme.Dark),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_UpdateExistingPreference_When_PreferenceExists()
    {
        var existingPreference = new UserPreference
        {
            Id = Guid.NewGuid(),
            UserProfileId = Guid.NewGuid(),
            Language = "en",
            Theme = Theme.Light,
            TimeZone = "UTC"
        };
        var profile = new UserProfile
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            UserPreference = existingPreference
        };

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
            new UpdateUserPreferenceCommand
            {
                UserId = _userId,
                Language = "mr",
                Theme = Theme.System,
                TimeZone = "Asia/Kolkata"
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        existingPreference.Language.Should().Be("mr");
        existingPreference.Theme.Should().Be(Theme.System);
        existingPreference.TimeZone.Should().Be("Asia/Kolkata");
    }

    [Fact]
    public async Task Handle_Should_PreserveExistingValues_When_NullFieldsProvided()
    {
        var existingPreference = new UserPreference
        {
            Id = Guid.NewGuid(),
            UserProfileId = Guid.NewGuid(),
            Language = "en",
            Theme = Theme.Dark,
            EmailNotifications = false
        };
        var profile = new UserProfile
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            UserPreference = existingPreference
        };

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
            new UpdateUserPreferenceCommand
            {
                UserId = _userId,
                Language = null,
                Theme = null
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        existingPreference.Language.Should().Be("en");
        existingPreference.Theme.Should().Be(Theme.Dark);
    }

    [Fact]
    public async Task Handle_Should_UpdateNotificationSettings_When_Provided()
    {
        var existingPreference = new UserPreference
        {
            Id = Guid.NewGuid(),
            UserProfileId = Guid.NewGuid(),
            EmailNotifications = true,
            PushNotifications = true,
            SmsNotifications = false,
            MarketingEmails = false
        };
        var profile = new UserProfile
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            UserPreference = existingPreference
        };

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
            new UpdateUserPreferenceCommand
            {
                UserId = _userId,
                EmailNotifications = false,
                PushNotifications = false,
                SmsNotifications = true,
                MarketingEmails = true
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        existingPreference.EmailNotifications.Should().BeFalse();
        existingPreference.PushNotifications.Should().BeFalse();
        existingPreference.SmsNotifications.Should().BeTrue();
        existingPreference.MarketingEmails.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_UpdateVisibilitySettings_When_Provided()
    {
        var existingPreference = new UserPreference
        {
            Id = Guid.NewGuid(),
            UserProfileId = Guid.NewGuid(),
            ProfileVisibility = true,
            ShowOnlineStatus = true
        };
        var profile = new UserProfile
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            UserPreference = existingPreference
        };

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
            new UpdateUserPreferenceCommand
            {
                UserId = _userId,
                ProfileVisibility = false,
                ShowOnlineStatus = false
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        existingPreference.ProfileVisibility.Should().BeFalse();
        existingPreference.ShowOnlineStatus.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Should_CallSaveChanges_When_Updated()
    {
        var profile = new UserProfile
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            UserPreference = new UserPreference { Id = Guid.NewGuid(), UserProfileId = Guid.NewGuid() }
        };

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
            new UpdateUserPreferenceCommand { UserId = _userId, Language = "hi" },
            CancellationToken.None);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnDto_When_UpdateSucceeds()
    {
        var profile = new UserProfile
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            UserPreference = new UserPreference
            {
                Id = Guid.NewGuid(),
                UserProfileId = Guid.NewGuid(),
                Language = "en"
            }
        };

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
            new UpdateUserPreferenceCommand
            {
                UserId = _userId,
                Language = "hi"
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Language.Should().Be("hi");
    }
}
