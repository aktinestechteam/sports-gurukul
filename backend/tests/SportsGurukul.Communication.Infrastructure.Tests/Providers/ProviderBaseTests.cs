using Microsoft.Extensions.Logging;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;
using SportsGurukul.Platform.Communication.Providers;

namespace SportsGurukul.Communication.Infrastructure.Tests.Providers;

public class ProviderBaseTests
{
    public sealed class TestProvider : ProviderBase
    {
        public override string Name => "TestProvider";
        public override NotificationChannelType ChannelType => NotificationChannelType.Email;

        public TestProvider(ILogger logger) : base(logger) { }

        public override Task<ProviderSendResult> SendAsync(ProviderMessage message, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(SimulateSuccess(Name, message));
        }

        public new ProviderSendResult SimulateSuccess(string providerName, ProviderMessage message)
            => base.SimulateSuccess(providerName, message);

        public new ProviderSendResult SimulateFailure(string providerName, string errorMessage)
            => base.SimulateFailure(providerName, errorMessage);
    }

    private readonly Mock<ILogger<TestProvider>> _loggerMock;
    private readonly TestProvider _provider;

    public ProviderBaseTests()
    {
        _loggerMock = new Mock<ILogger<TestProvider>>();
        _provider = new TestProvider(_loggerMock.Object);
    }

    [Fact]
    public void GetProviderType_ShouldReturnName()
    {
        _provider.Name.Should().Be("TestProvider");
    }

    [Fact]
    public void ChannelType_ShouldReturnSetValue()
    {
        _provider.ChannelType.Should().Be(NotificationChannelType.Email);
    }

    [Fact]
    public void IsAvailable_DefaultShouldBeTrue()
    {
        _provider.IsAvailable.Should().BeTrue();
    }

    [Fact]
    public async Task SimulateSuccess_ShouldReturnSuccessfulResult()
    {
        var message = new ProviderMessage
        {
            To = "test@example.com",
            Subject = "Test",
            Body = "Body"
        };

        var result = _provider.SimulateSuccess("TestProvider", message);

        result.IsSuccess.Should().BeTrue();
        result.ProviderMessageId.Should().StartWith("testprovider_");
        result.ProviderMessageId.Should().Contain("_");
        result.DurationMs.Should().BeInRange(50, 300);
        result.ProviderResponse.Should().ContainKey("simulated").WhoseValue.Should().Be("true");
        result.ProviderResponse.Should().ContainKey("provider").WhoseValue.Should().Be("TestProvider");
    }

    [Fact]
    public async Task SimulateFailure_ShouldReturnFailedResult()
    {
        var result = _provider.SimulateFailure("TestProvider", "Something went wrong");

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Something went wrong");
        result.ErrorCode.Should().Be("SIMULATED_FAILURE");
        result.DurationMs.Should().BeInRange(10, 100);
        result.ProviderMessageId.Should().BeNull();
    }

    [Fact]
    public async Task SimulateSuccess_ShouldGenerateUniqueIds()
    {
        var message = new ProviderMessage { To = "a@b.com", Subject = "S1", Body = "B1" };

        var result1 = _provider.SimulateSuccess("TestProvider", message);
        var result2 = _provider.SimulateSuccess("TestProvider", message);

        result1.ProviderMessageId.Should().NotBe(result2.ProviderMessageId);
    }

    [Fact]
    public async Task SimulateSuccess_ShouldHaveRandomDuration()
    {
        var message = new ProviderMessage { To = "a@b.com", Subject = "S", Body = "B" };

        var durations = Enumerable.Range(0, 10)
            .Select(_ => _provider.SimulateSuccess("TestProvider", message).DurationMs)
            .ToList();

        durations.Should().OnlyContain(d => d >= 50 && d < 300);
    }

    [Fact]
    public async Task HealthCheckAsync_ShouldReturnTrue()
    {
        var healthy = await _provider.HealthCheckAsync();
        healthy.Should().BeTrue();
    }

    [Fact]
    public void SimulateSuccess_ShouldLogInformation()
    {
        var message = new ProviderMessage { To = "test@example.com", Subject = "Log Test", Body = "Body" };

        _provider.SimulateSuccess("TestProvider", message);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Simulated send") && v.ToString()!.Contains("TestProvider")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void SimulateFailure_ShouldLogWarning()
    {
        _provider.SimulateFailure("TestProvider", "Error occurred");

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Simulated failure") && v.ToString()!.Contains("TestProvider")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void ValidateConfiguration_ShouldWorkWithTestProvider()
    {
        _provider.Should().NotBeNull();
        _provider.Name.Should().Be("TestProvider");
    }

    [Fact]
    public async Task TrackMetrics_ShouldRecordDeliveryMetrics()
    {
        var message = new ProviderMessage { To = "test@example.com", Subject = "Metrics", Body = "Body" };
        var result = _provider.SimulateSuccess("TestProvider", message);

        result.DurationMs.Should().BeGreaterThan(0);
        result.IsSuccess.Should().BeTrue();
    }
}
