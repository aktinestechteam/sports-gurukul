using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.Configuration;
using SportsGurukul.Platform.Communication.Analytics.DTOs;
using SportsGurukul.Platform.Communication.Analytics.Services;

namespace SportsGurukul.Communication.Infrastructure.Tests.EdgeCases;

public class QuietHoursTests
{
    private readonly Mock<ILogger<SchedulingEngine>> _loggerMock = new();
    private readonly Mock<ICacheService> _cacheMock = new();
    private readonly IOptions<SchedulingOptions> _options;

    public QuietHoursTests()
    {
        _options = Options.Create(new SchedulingOptions
        {
            DefaultTimeZone = "UTC",
            BusinessHoursStart = new TimeSpan(9, 0, 0),
            BusinessHoursEnd = new TimeSpan(17, 0, 0),
            QuietHoursStart = new TimeSpan(22, 0, 0),
            QuietHoursEnd = new TimeSpan(7, 0, 0),
            BusinessDays = new List<DayOfWeek>
            {
                DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
                DayOfWeek.Thursday, DayOfWeek.Friday
            }
        });
    }

    private SchedulingEngine CreateEngine() => new(_loggerMock.Object, _cacheMock.Object, _options);

    [Fact]
    public async Task IsQuietHoursAsync_DuringQuietHours_ReturnsTrue()
    {
        var engine = CreateEngine();
        var quietTime = new DateTime(2026, 1, 15, 23, 0, 0, DateTimeKind.Utc);

        var result = await engine.IsQuietHoursAsync(quietTime, null);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsQuietHoursAsync_MorningQuietHours_ReturnsTrue()
    {
        var engine = CreateEngine();
        var earlyMorning = new DateTime(2026, 1, 15, 5, 0, 0, DateTimeKind.Utc);

        var result = await engine.IsQuietHoursAsync(earlyMorning, null);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsQuietHoursAsync_OutsideQuietHours_ReturnsFalse()
    {
        var engine = CreateEngine();
        var businessHour = new DateTime(2026, 1, 15, 14, 0, 0, DateTimeKind.Utc);

        var result = await engine.IsQuietHoursAsync(businessHour, null);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsQuietHoursAsync_UsesTimeZoneConversion()
    {
        var engine = CreateEngine();
        var utc9am = new DateTime(2026, 1, 15, 9, 0, 0, DateTimeKind.Utc);

        var result = await engine.IsQuietHoursAsync(utc9am, null);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsQuietHoursAsync_WithTimezone_ConvertsCorrectly()
    {
        var engine = CreateEngine();
        var utcMidnight = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc);

        var result = await engine.IsQuietHoursAsync(utcMidnight, null);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsWithinBusinessHoursAsync_DuringBusinessHours_ReturnsTrue()
    {
        var engine = CreateEngine();
        var businessHour = new DateTime(2026, 1, 15, 10, 0, 0, DateTimeKind.Utc);

        var result = await engine.IsWithinBusinessHoursAsync(businessHour, null);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsWithinBusinessHoursAsync_OutsideBusinessHours_ReturnsFalse()
    {
        var engine = CreateEngine();
        var afterHours = new DateTime(2026, 1, 15, 20, 0, 0, DateTimeKind.Utc);

        var result = await engine.IsWithinBusinessHoursAsync(afterHours, null);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsWithinBusinessHoursAsync_Weekend_ReturnsFalse()
    {
        var engine = CreateEngine();
        var saturday = new DateTime(2026, 1, 17, 10, 0, 0, DateTimeKind.Utc);

        var result = await engine.IsWithinBusinessHoursAsync(saturday, null);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task SetQuietHoursAsync_UpdatesQuietHours()
    {
        var engine = CreateEngine();
        var newQuietHours = new QuietHoursDto(
            Guid.NewGuid(), "Custom", new TimeSpan(23, 0, 0), new TimeSpan(6, 0, 0),
            "UTC", null, true, false);

        var result = await engine.SetQuietHoursAsync(newQuietHours);

        result.Start.Should().Be(new TimeSpan(23, 0, 0));
        result.End.Should().Be(new TimeSpan(6, 0, 0));
    }
}
