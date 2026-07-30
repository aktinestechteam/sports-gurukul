using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Platform.Communication.Configuration;
using SportsGurukul.Platform.Communication.Delivery;
using SportsGurukul.Communication.Infrastructure.Tests.Fixtures;

namespace SportsGurukul.Communication.Infrastructure.Tests.Delivery;

public class CircuitBreakerTests
{
    [Fact]
    public void GetState_ReturnsClosed_Initially()
    {
        var cb = new CircuitBreaker(
            TestDataFactory.CreateOptions(),
            Mock.Of<ILogger<CircuitBreaker>>());
        var state = cb.GetState();
        state.Should().Be(CircuitState.Closed);
    }

    [Fact]
    public void GetState_ReturnsOpen_AfterThresholdFailures()
    {
        var options = TestDataFactory.CreateOptions(o =>
        {
            o.CircuitBreaker.FailureThreshold = 3;
            o.CircuitBreaker.SuccessThreshold = 2;
            o.CircuitBreaker.OpenDurationSeconds = 60;
        });
        var cb = new CircuitBreaker(options, Mock.Of<ILogger<CircuitBreaker>>());

        for (int i = 0; i < 3; i++)
            cb.RecordFailure();

        var state = cb.GetState();
        state.Should().Be(CircuitState.Open);
    }

    [Fact]
    public void RecordFailure_IncrementsCounter()
    {
        var options = TestDataFactory.CreateOptions(o =>
        {
            o.CircuitBreaker.FailureThreshold = 3;
            o.CircuitBreaker.SuccessThreshold = 2;
            o.CircuitBreaker.OpenDurationSeconds = 60;
        });
        var cb = new CircuitBreaker(options, Mock.Of<ILogger<CircuitBreaker>>());

        cb.RecordFailure();
        cb.RecordFailure();
        cb.GetState().Should().Be(CircuitState.Closed);

        cb.RecordFailure();
        cb.GetState().Should().Be(CircuitState.Open);
    }

    [Fact]
    public void RecordSuccess_ResetsCounter_InClosedState()
    {
        var options = TestDataFactory.CreateOptions(o =>
        {
            o.CircuitBreaker.FailureThreshold = 3;
            o.CircuitBreaker.SuccessThreshold = 2;
            o.CircuitBreaker.OpenDurationSeconds = 60;
        });
        var cb = new CircuitBreaker(options, Mock.Of<ILogger<CircuitBreaker>>());

        cb.RecordFailure();
        cb.RecordFailure();
        cb.RecordSuccess();
        cb.RecordFailure();
        cb.GetState().Should().Be(CircuitState.Closed);

        cb.RecordFailure();
        cb.GetState().Should().Be(CircuitState.Open);
    }

    [Fact]
    public void HalfOpen_AllowsTrialRequest()
    {
        var options = TestDataFactory.CreateOptions(o =>
        {
            o.CircuitBreaker.FailureThreshold = 3;
            o.CircuitBreaker.SuccessThreshold = 2;
            o.CircuitBreaker.OpenDurationSeconds = 0;
        });
        var cb = new CircuitBreaker(options, Mock.Of<ILogger<CircuitBreaker>>());

        for (int i = 0; i < 3; i++)
            cb.RecordFailure();

        cb.GetState().Should().Be(CircuitState.HalfOpen);
    }

    [Fact]
    public void HalfOpenSuccess_ClosesCircuit()
    {
        var options = TestDataFactory.CreateOptions(o =>
        {
            o.CircuitBreaker.FailureThreshold = 3;
            o.CircuitBreaker.SuccessThreshold = 2;
            o.CircuitBreaker.OpenDurationSeconds = 0;
        });
        var cb = new CircuitBreaker(options, Mock.Of<ILogger<CircuitBreaker>>());

        for (int i = 0; i < 3; i++)
            cb.RecordFailure();

        cb.GetState(); // transitions to HalfOpen (0s timeout)
        cb.RecordSuccess();
        cb.RecordSuccess();

        cb.GetState().Should().Be(CircuitState.Closed);
    }

    [Fact]
    public void HalfOpenFailure_ReopensCircuit()
    {
        var options = TestDataFactory.CreateOptions(o =>
        {
            o.CircuitBreaker.FailureThreshold = 3;
            o.CircuitBreaker.SuccessThreshold = 2;
            o.CircuitBreaker.OpenDurationSeconds = 0;
        });
        var cb = new CircuitBreaker(options, Mock.Of<ILogger<CircuitBreaker>>());

        for (int i = 0; i < 3; i++)
            cb.RecordFailure();

        cb.GetState(); // transitions to HalfOpen

        cb.RecordFailure();
        cb.State.Should().Be(CircuitState.Open);
    }

    [Fact]
    public void Reset_ReturnsToClosedState()
    {
        var options = TestDataFactory.CreateOptions(o =>
        {
            o.CircuitBreaker.FailureThreshold = 1;
            o.CircuitBreaker.SuccessThreshold = 1;
            o.CircuitBreaker.OpenDurationSeconds = 60;
        });
        var cb = new CircuitBreaker(options, Mock.Of<ILogger<CircuitBreaker>>());

        for (int i = 0; i < 5; i++)
            cb.RecordFailure();

        cb.GetState().Should().Be(CircuitState.Open);

        cb.Reset();

        cb.GetState().Should().Be(CircuitState.Closed);
    }

    [Fact]
    public void CircuitBreaker_TransitionsToHalfOpen_AfterTimeoutPeriod()
    {
        var shortTimeoutOptions = TestDataFactory.CreateOptions(o =>
        {
            o.CircuitBreaker.FailureThreshold = 1;
            o.CircuitBreaker.SuccessThreshold = 1;
            o.CircuitBreaker.OpenDurationSeconds = 0;
        });
        var breaker = new CircuitBreaker(shortTimeoutOptions, Mock.Of<ILogger<CircuitBreaker>>());

        breaker.RecordFailure();

        var state = breaker.GetState();
        state.Should().Be(CircuitState.HalfOpen);
    }
}
