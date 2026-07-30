using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Platform.Communication.Configuration;

namespace SportsGurukul.Platform.Communication.Delivery;

public enum CircuitState
{
    Closed,
    Open,
    HalfOpen
}

public class CircuitBreaker
{
    private readonly CircuitBreakerOptions _options;
    private readonly ILogger<CircuitBreaker> _logger;
    private readonly object _lock = new();

    private CircuitState _state = CircuitState.Closed;
    private int _failureCount;
    private int _successCount;
    private DateTime _lastFailureTime;
    private DateTime _openTime;

    public CircuitState State => _state;

    public CircuitBreaker(IOptions<CommunicationOptions> options, ILogger<CircuitBreaker> logger)
    {
        _options = options.Value.CircuitBreaker;
        _logger = logger;
    }

    public CircuitState GetState()
    {
        lock (_lock)
        {
            if (_state == CircuitState.Open && (DateTime.UtcNow - _openTime).TotalSeconds >= _options.OpenDurationSeconds)
            {
                _state = CircuitState.HalfOpen;
                _successCount = 0;
                _logger.LogInformation("Circuit breaker transitioning from OPEN to HALF-OPEN");
            }

            return _state;
        }
    }

    public void RecordFailure()
    {
        lock (_lock)
        {
            _failureCount++;
            _lastFailureTime = DateTime.UtcNow;

            if (_state == CircuitState.Closed && _failureCount >= _options.FailureThreshold)
            {
                _state = CircuitState.Open;
                _openTime = DateTime.UtcNow;
                _logger.LogWarning("Circuit breaker OPEN after {Count} failures", _failureCount);
            }
            else if (_state == CircuitState.HalfOpen)
            {
                _state = CircuitState.Open;
                _openTime = DateTime.UtcNow;
                _logger.LogWarning("Circuit breaker returned to OPEN after failure in HALF-OPEN state");
            }
        }
    }

    public void RecordSuccess()
    {
        lock (_lock)
        {
            if (_state == CircuitState.HalfOpen)
            {
                _successCount++;
                if (_successCount >= _options.SuccessThreshold)
                {
                    _state = CircuitState.Closed;
                    _failureCount = 0;
                    _successCount = 0;
                    _logger.LogInformation("Circuit breaker CLOSED after {Count} successes in HALF-OPEN state", _successCount);
                }
            }
            else if (_state == CircuitState.Closed)
            {
                _failureCount = Math.Max(0, _failureCount - 1);
            }
        }
    }

    public void Reset()
    {
        lock (_lock)
        {
            _state = CircuitState.Closed;
            _failureCount = 0;
            _successCount = 0;
            _logger.LogInformation("Circuit breaker manually reset to CLOSED");
        }
    }
}
