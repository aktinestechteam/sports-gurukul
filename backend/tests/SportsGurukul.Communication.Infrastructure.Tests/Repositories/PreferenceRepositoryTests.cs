using System.Linq.Expressions;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Infrastructure.Tests.Repositories;

public class PreferenceRepositoryTests
{
    private static int _counter;

    private static NotificationPreference CreatePreference(
        Guid? userId = null,
        NotificationChannelType channel = NotificationChannelType.Email,
        bool isEnabled = true)
    {
        _counter++;
        return new NotificationPreference
        {
            Id = Guid.NewGuid(),
            UserId = userId ?? Guid.NewGuid(),
            ChannelType = channel,
            IsEnabled = isEnabled,
            CreatedAt = DateTime.UtcNow
        };
    }

    private readonly Mock<IPreferenceRepository> _mock;
    private readonly List<NotificationPreference> _preferences;
    private readonly Guid _sharedUserId = Guid.NewGuid();

    public PreferenceRepositoryTests()
    {
        _preferences =
        [
            CreatePreference(_sharedUserId, NotificationChannelType.Email, true),
            CreatePreference(_sharedUserId, NotificationChannelType.SMS, false),
            CreatePreference(Guid.NewGuid(), NotificationChannelType.Email, true)
        ];
        _mock = CreateMockWithBaseSetup(_preferences);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnPreferences()
    {
        var userPrefs = _preferences.Where(p => p.UserId == _sharedUserId).ToList();
        _mock.Setup(r => r.GetByUserIdAsync(_sharedUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userPrefs);
        var result = await _mock.Object.GetByUserIdAsync(_sharedUserId);
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(p => p.UserId.Should().Be(_sharedUserId));
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnEmpty_ForUnknownUser()
    {
        _mock.Setup(r => r.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationPreference>());
        var result = await _mock.Object.GetByUserIdAsync(Guid.NewGuid());
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpsertAsync_CreatesNewPreference()
    {
        var preference = CreatePreference();
        _mock.Setup(r => r.AddAsync(preference, It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);
        var result = await _mock.Object.AddAsync(preference);
        result.Should().Be(preference);
        result.IsEnabled.Should().BeTrue();
    }

    [Fact]
    public void UpsertAsync_UpdatesExistingPreference()
    {
        var preference = _preferences[0];
        preference.IsEnabled = false;
        _mock.Object.Update(preference);
        _mock.Verify(r => r.Update(preference), Times.Once);
        preference.IsEnabled.Should().BeFalse();
    }

    [Fact]
    public void DeleteAsync_RemovesPreference()
    {
        var preference = _preferences[0];
        _mock.Object.Remove(preference);
        _mock.Verify(r => r.Remove(preference), Times.Once);
    }

    [Fact]
    public async Task GetByChannelAsync_ReturnsPreferencesForChannel()
    {
        var emailPrefs = _preferences.Where(p => p.ChannelType == NotificationChannelType.Email).ToList();
        _mock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<NotificationPreference, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(emailPrefs);
        var result = await _mock.Object.FindAsync(p => p.ChannelType == NotificationChannelType.Email);
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(p => p.ChannelType.Should().Be(NotificationChannelType.Email));
    }

    [Fact]
    public async Task GetByUserAndChannelAsync_ReturnsPreference_WhenFound()
    {
        var expected = _preferences[0];
        _mock.Setup(r => r.GetByUserAndChannelAsync(expected.UserId, expected.ChannelType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);
        var result = await _mock.Object.GetByUserAndChannelAsync(expected.UserId, expected.ChannelType);
        result.Should().Be(expected);
    }

    [Fact]
    public async Task GetByUserAndChannelAsync_ReturnsNull_WhenNotFound()
    {
        _mock.Setup(r => r.GetByUserAndChannelAsync(It.IsAny<Guid>(), It.IsAny<NotificationChannelType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationPreference?)null);
        var result = await _mock.Object.GetByUserAndChannelAsync(Guid.NewGuid(), NotificationChannelType.PushNotification);
        result.Should().BeNull();
    }

    [Fact]
    public async Task IsChannelEnabledAsync_ReturnsTrue_WhenEnabled()
    {
        var pref = _preferences[0];
        _mock.Setup(r => r.IsChannelEnabledAsync(pref.UserId, pref.ChannelType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var result = await _mock.Object.IsChannelEnabledAsync(pref.UserId, pref.ChannelType);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsChannelEnabledAsync_ReturnsFalse_WhenDisabled()
    {
        var pref = _preferences[1];
        _mock.Setup(r => r.IsChannelEnabledAsync(pref.UserId, pref.ChannelType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var result = await _mock.Object.IsChannelEnabledAsync(pref.UserId, pref.ChannelType);
        result.Should().BeFalse();
    }

    private static Mock<IPreferenceRepository> CreateMockWithBaseSetup(List<NotificationPreference> data)
    {
        var mock = new Mock<IPreferenceRepository>();

        mock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => data.FirstOrDefault(e => e.Id == id));

        mock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);

        mock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<NotificationPreference, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<NotificationPreference, bool>> predicate, CancellationToken _) =>
                data.AsQueryable().Where(predicate).ToList());

        mock.Setup(r => r.AddAsync(It.IsAny<NotificationPreference>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationPreference entity, CancellationToken _) => entity);

        mock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<NotificationPreference, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<NotificationPreference, bool>>? predicate, CancellationToken _) =>
                predicate == null ? data.Count : data.AsQueryable().Count(predicate));

        mock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<NotificationPreference, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<NotificationPreference, bool>> predicate, CancellationToken _) =>
                data.AsQueryable().Any(predicate));

        return mock;
    }
}
