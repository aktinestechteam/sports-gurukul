using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Platform.Communication.Abstractions;
using SportsGurukul.Platform.Communication.Configuration;
using SportsGurukul.Platform.Communication.Delivery;

namespace SportsGurukul.Communication.Infrastructure.Tests.EdgeCases;

public class MaximumRetryTests
{
    private readonly Mock<IDeliveryRepository> _deliveryRepoMock = new();
    private readonly IOptions<CommunicationOptions> _options;
    private readonly Mock<ILogger<RetryEngine>> _loggerMock = new();

    public MaximumRetryTests()
    {
        _options = Options.Create(new CommunicationOptions
        {
            Retry = new RetryOptions
            {
                MaxRetries = 3,
                BaseDelayMs = 10,
                MaxDelayMs = 100,
                BackoffMultiplier = 2.0,
                JitterEnabled = false
            }
        });
    }

    [Fact]
    public async Task ExecuteWithRetryAsync_ExceedsMaxRetries_MovesToDeadLetter()
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
        var attemptCount = 0;

        var result = await engine.ExecuteWithRetryAsync(() =>
        {
            attemptCount++;
            return Task.FromResult(new ProviderSendResult
            {
                IsSuccess = false,
                ErrorMessage = "Provider unavailable",
                ErrorCode = "PROVIDER_ERROR"
            });
        }, Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Max retries");
        result.ErrorCode.Should().Be("MAX_RETRIES_EXCEEDED");
        attemptCount.Should().Be(4);
    }

    [Fact]
    public async Task ExecuteWithRetryAsync_SucceedsOnFirstAttempt_NoRetries()
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
        var attemptCount = 0;

        var result = await engine.ExecuteWithRetryAsync(() =>
        {
            attemptCount++;
            return Task.FromResult(new ProviderSendResult
            {
                IsSuccess = true,
                ProviderMessageId = "msg-001"
            });
        }, Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.ProviderMessageId.Should().Be("msg-001");
        attemptCount.Should().Be(1);
    }

    [Fact]
    public async Task ExecuteWithRetryAsync_RetriesOnFailure_ThenSucceeds()
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
        var attemptCount = 0;

        var result = await engine.ExecuteWithRetryAsync(() =>
        {
            attemptCount++;
            if (attemptCount < 3)
                return Task.FromResult(new ProviderSendResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Temporary failure"
                });
            return Task.FromResult(new ProviderSendResult
            {
                IsSuccess = true,
                ProviderMessageId = "msg-002"
            });
        }, Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.ProviderMessageId.Should().Be("msg-002");
        attemptCount.Should().Be(3);
    }

    [Fact]
    public async Task ExecuteWithRetryAsync_CircuitBreakerOpen_SkipsSending()
    {
        var cbOptions = Options.Create(new CommunicationOptions
        {
            CircuitBreaker = new CircuitBreakerOptions
            {
                FailureThreshold = 1,
                SuccessThreshold = 1,
                OpenDurationSeconds = 60
            }
        });
        var cb = new CircuitBreaker(cbOptions, Mock.Of<ILogger<CircuitBreaker>>());
        cb.RecordFailure();

        var engine = new RetryEngine(_deliveryRepoMock.Object, cb, _options, _loggerMock.Object);
        var wasCalled = false;

        var result = await engine.ExecuteWithRetryAsync(() =>
        {
            wasCalled = true;
            return Task.FromResult(new ProviderSendResult { IsSuccess = true });
        }, Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("CIRCUIT_OPEN");
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteWithRetryAsync_ExceptionDuringSend_IsHandled()
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

        var result = await engine.ExecuteWithRetryAsync(() =>
            throw new InvalidOperationException("Network error"),
            Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Max retries");
    }

    [Fact]
    public void CircuitBreaker_DefaultState_IsClosed()
    {
        var cb = new CircuitBreaker(
            Options.Create(new CommunicationOptions()),
            Mock.Of<ILogger<CircuitBreaker>>());

        cb.GetState().Should().Be(CircuitState.Closed);
    }

    [Fact]
    public void CircuitBreaker_AfterFailureThreshold_TransitionsToOpen()
    {
        var options = Options.Create(new CommunicationOptions
        {
            CircuitBreaker = new CircuitBreakerOptions
            {
                FailureThreshold = 3,
                SuccessThreshold = 2,
                OpenDurationSeconds = 30
            }
        });
        var cb = new CircuitBreaker(options, Mock.Of<ILogger<CircuitBreaker>>());

        cb.RecordFailure();
        cb.RecordFailure();
        cb.RecordFailure();

        cb.GetState().Should().Be(CircuitState.Open);
    }

    [Fact]
    public void CircuitBreaker_Reset_ReturnsToClosed()
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
        cb.Reset();

        cb.GetState().Should().Be(CircuitState.Closed);
    }
}
