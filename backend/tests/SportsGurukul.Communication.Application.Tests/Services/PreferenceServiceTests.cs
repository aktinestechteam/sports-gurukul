using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.BusinessRules;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Application.Features.NotificationManagement.Services;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Application.Tests.Services;

public class PreferenceServiceTests
{
    private readonly Mock<IPreferenceRepository> _preferenceRepoMock;
    private readonly Mock<INotificationRepository> _notificationRepoMock;
    private readonly Mock<IBusinessRuleValidator> _ruleValidatorMock;
    private readonly Mock<ILogger<PreferenceService>> _loggerMock;
    private readonly PreferenceService _service;

    public PreferenceServiceTests()
    {
        _preferenceRepoMock = new Mock<IPreferenceRepository>();
        _notificationRepoMock = new Mock<INotificationRepository>();
        _ruleValidatorMock = new Mock<IBusinessRuleValidator>();
        _loggerMock = new Mock<ILogger<PreferenceService>>();
        _service = new PreferenceService(
            _preferenceRepoMock.Object,
            _notificationRepoMock.Object,
            _ruleValidatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetByUserAsync_ShouldReturnPreferences()
    {
        var userId = Guid.NewGuid();
        var entities = new List<NotificationPreference>
        {
            new() { Id = Guid.NewGuid(), UserId = userId, ChannelType = NotificationChannelType.Email, IsEnabled = true, QuietHoursStart = new TimeOnly(9, 0), QuietHoursEnd = new TimeOnly(18, 0), MaxPerDay = 10, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), UserId = userId, ChannelType = NotificationChannelType.SMS, IsEnabled = false, CreatedAt = DateTime.UtcNow },
        };

        _preferenceRepoMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        var result = await _service.GetByUserAsync(userId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().Contain(p => p.ChannelType == NotificationChannelType.Email && p.IsEnabled);
        result.Value.Should().Contain(p => p.ChannelType == NotificationChannelType.SMS && !p.IsEnabled);
    }

    [Fact]
    public async Task GetByUserAsync_ShouldReturnEmpty_WhenNoPreferences()
    {
        var userId = Guid.NewGuid();
        _preferenceRepoMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationPreference>());

        var result = await _service.GetByUserAsync(userId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateNewPreference()
    {
        var request = new CreatePreferenceRequest(Guid.NewGuid(), NotificationChannelType.Email, true, new TimeOnly(9, 0), new TimeOnly(18, 0), 10);

        _ruleValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        _preferenceRepoMock.Setup(r => r.GetByUserAndChannelAsync(request.UserId, request.ChannelType, It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationPreference?)null);

        NotificationPreference? addedEntity = null;
        _preferenceRepoMock.Setup(r => r.AddAsync(It.IsAny<NotificationPreference>(), It.IsAny<CancellationToken>()))
            .Callback<NotificationPreference, CancellationToken>((e, _) => addedEntity = e)
            .ReturnsAsync((NotificationPreference e, CancellationToken _) => e);

        var result = await _service.CreateAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value!.IsEnabled.Should().BeTrue();
        addedEntity.Should().NotBeNull();
        addedEntity!.UserId.Should().Be(request.UserId);
        addedEntity.ChannelType.Should().Be(NotificationChannelType.Email);
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenAlreadyExists()
    {
        var request = new CreatePreferenceRequest(Guid.NewGuid(), NotificationChannelType.Email, true, null, null, null);
        var existing = new NotificationPreference { Id = Guid.NewGuid(), UserId = request.UserId, ChannelType = request.ChannelType, IsEnabled = true, CreatedAt = DateTime.UtcNow };

        _ruleValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        _preferenceRepoMock.Setup(r => r.GetByUserAndChannelAsync(request.UserId, request.ChannelType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await _service.CreateAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already exists");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateChannels()
    {
        var userId = Guid.NewGuid();
        var entity = new NotificationPreference
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ChannelType = NotificationChannelType.Email,
            IsEnabled = true,
            MaxPerDay = 5,
            CreatedAt = DateTime.UtcNow,
        };

        _preferenceRepoMock.Setup(r => r.GetByUserAndChannelAsync(userId, NotificationChannelType.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        _ruleValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<UpdatePreferenceRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var request = new UpdatePreferenceRequest(userId, NotificationChannelType.Email, false, new TimeOnly(10, 0), new TimeOnly(20, 0), 3);
        var result = await _service.UpdateAsync(request);

        result.IsSuccess.Should().BeTrue();
        entity.IsEnabled.Should().BeFalse();
        entity.MaxPerDay.Should().Be(3);
        entity.QuietHoursStart.Should().Be(new TimeOnly(10, 0));
        entity.QuietHoursEnd.Should().Be(new TimeOnly(20, 0));
    }

    [Fact]
    public async Task SubscribeAsync_ShouldAddSubscription()
    {
        var request = new SubscribeRequest(Guid.NewGuid(), "Tournament", Guid.NewGuid(), NotificationChannelType.Email, "MatchStart");

        var result = await _service.SubscribeAsync(request);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task UnsubscribeAsync_ShouldRemoveSubscription()
    {
        var request = new UnsubscribeRequest(Guid.NewGuid(), "Tournament", Guid.NewGuid(), NotificationChannelType.Email, "MatchStart");

        var result = await _service.UnsubscribeAsync(request);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task MuteChannelAsync_ShouldMuteChannel()
    {
        var userId = Guid.NewGuid();
        var entity = new NotificationPreference
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ChannelType = NotificationChannelType.Email,
            IsEnabled = true,
            CreatedAt = DateTime.UtcNow,
        };

        _preferenceRepoMock.Setup(r => r.GetByUserAndChannelAsync(userId, NotificationChannelType.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _service.MuteChannelAsync(userId, NotificationChannelType.Email);

        result.IsSuccess.Should().BeTrue();
        entity.IsEnabled.Should().BeFalse();
    }

    [Fact]
    public async Task MuteChannelAsync_ShouldFail_WhenNotFound()
    {
        var userId = Guid.NewGuid();
        _preferenceRepoMock.Setup(r => r.GetByUserAndChannelAsync(userId, NotificationChannelType.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationPreference?)null);

        var result = await _service.MuteChannelAsync(userId, NotificationChannelType.Email);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }
}
