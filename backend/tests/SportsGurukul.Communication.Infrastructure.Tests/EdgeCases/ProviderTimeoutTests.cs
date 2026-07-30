using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Platform.Communication.Abstractions;
using SportsGurukul.Platform.Communication.Configuration;
using SportsGurukul.Platform.Communication.Delivery;

namespace SportsGurukul.Communication.Infrastructure.Tests.EdgeCases;

public class ProviderTimeoutTests
{
    private readonly Mock<IDeliveryRepository> _deliveryRepoMock = new();
    private readonly IOptions<CommunicationOptions> _options;
    private readonly Mock<ILogger<RetryEngine>> _loggerMock = new();

    public ProviderTimeoutTests()
    {
        _options = Options.Create(new CommunicationOptions
        {
            Retry = new RetryOptions
            {
                MaxRetries = 1,
                BaseDelayMs = 5,
                MaxDelayMs = 100,
                BackoffMultiplier = 2.0,
                JitterEnabled = false
            }
        });
    }

    [Fact]
    public async Task ExecuteWithRetryAsync_Timeout_ReturnsFailure()
    {
        var cb = new CircuitBreaker(
            Options.Create(new CommunicationOptions
            {
                CircuitBreaker = new CircuitBreakerOptions
                {
                    FailureThreshold = 100,
                    SuccessThreshold = 1,
                    OpenDurationSeconds = 60
                }
            }),
            Mock.Of<ILogger<CircuitBreaker>>());
        var engine = new RetryEngine(_deliveryRepoMock.Object, cb, _options, _loggerMock.Object);

        var result = await engine.ExecuteWithRetryAsync(
            () => Task.FromException<ProviderSendResult>(new TimeoutException("Provider timed out")),
            Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Max retries");
    }

    [Fact]
    public async Task ExecuteWithRetryAsync_TimeoutTriggersFallback()
    {
        var cb = new CircuitBreaker(
            Options.Create(new CommunicationOptions
            {
                CircuitBreaker = new CircuitBreakerOptions
                {
                    FailureThreshold = 100,
                    SuccessThreshold = 1,
                    OpenDurationSeconds = 60
                }
            }),
            Mock.Of<ILogger<CircuitBreaker>>());
        var engine = new RetryEngine(_deliveryRepoMock.Object, cb, _options, _loggerMock.Object);

        var attempts = 0;
        var result = await engine.ExecuteWithRetryAsync(() =>
        {
            attempts++;
            if (attempts == 1)
                throw new TimeoutException("Provider timed out");
            return Task.FromResult(new ProviderSendResult
            {
                IsSuccess = true,
                ProviderMessageId = "fallback-msg"
            });
        }, Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.ProviderMessageId.Should().Be("fallback-msg");
        attempts.Should().Be(2);
    }

    [Fact]
    public void CircuitBreaker_OpensOnRepeatedTimeouts()
    {
        var options = Options.Create(new CommunicationOptions
        {
            CircuitBreaker = new CircuitBreakerOptions
            {
                FailureThreshold = 3,
                SuccessThreshold = 2,
                OpenDurationSeconds = 60
            }
        });
        var cb = new CircuitBreaker(options, Mock.Of<ILogger<CircuitBreaker>>());

        cb.RecordFailure();
        cb.RecordFailure();
        cb.RecordFailure();

        cb.GetState().Should().Be(CircuitState.Open);
    }

    [Fact]
    public void CircuitBreaker_TransitionsToHalfOpen_AfterDuration()
    {
        var options = Options.Create(new CommunicationOptions
        {
            CircuitBreaker = new CircuitBreakerOptions
            {
                FailureThreshold = 1,
                SuccessThreshold = 1,
                OpenDurationSeconds = 0
            }
        });
        var cb = new CircuitBreaker(options, Mock.Of<ILogger<CircuitBreaker>>());

        cb.RecordFailure();

        var state = cb.GetState();

        state.Should().Be(CircuitState.HalfOpen);
    }

    [Fact]
    public void CircuitBreaker_HalfOpenSuccess_Closes()
    {
        var options = Options.Create(new CommunicationOptions
        {
            CircuitBreaker = new CircuitBreakerOptions
            {
                FailureThreshold = 1,
                SuccessThreshold = 1,
                OpenDurationSeconds = 0
            }
        });
        var cb = new CircuitBreaker(options, Mock.Of<ILogger<CircuitBreaker>>());

        cb.RecordFailure();
        cb.GetState();
        cb.RecordSuccess();

        cb.GetState().Should().Be(CircuitState.Closed);
    }

    [Fact]
    public void CircuitBreaker_HalfOpenFailure_Reopens()
    {
        var options = Options.Create(new CommunicationOptions
        {
            CircuitBreaker = new CircuitBreakerOptions
            {
                FailureThreshold = 1,
                SuccessThreshold = 1,
                OpenDurationSeconds = 0
            }
        });
        var cb = new CircuitBreaker(options, Mock.Of<ILogger<CircuitBreaker>>());

        cb.RecordFailure();
        cb.GetState();
        cb.RecordFailure();

        cb.State.Should().Be(CircuitState.Open);
    }

    [Fact]
    public async Task ExecuteWithRetryAsync_ConsecutiveTimeouts_ExhaustRetries()
    {
        var cb = new CircuitBreaker(
            Options.Create(new CommunicationOptions
            {
                CircuitBreaker = new CircuitBreakerOptions
                {
                    FailureThreshold = 100,
                    SuccessThreshold = 1,
                    OpenDurationSeconds = 60
                }
            }),
            Mock.Of<ILogger<CircuitBreaker>>());
        var engine = new RetryEngine(_deliveryRepoMock.Object, cb, _options, _loggerMock.Object);

        var result = await engine.ExecuteWithRetryAsync(
            () => Task.FromException<ProviderSendResult>(new TimeoutException("Timeout")),
            Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Max retries");
    }
}
