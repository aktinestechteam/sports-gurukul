using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Abstractions;
using SportsGurukul.Platform.Communication.Configuration;

namespace SportsGurukul.Platform.Communication.Delivery;

public class RetryEngine
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly CircuitBreaker _circuitBreaker;
    private readonly RetryOptions _options;
    private readonly ILogger<RetryEngine> _logger;

    public RetryEngine(
        IDeliveryRepository deliveryRepository,
        CircuitBreaker circuitBreaker,
        IOptions<CommunicationOptions> options,
        ILogger<RetryEngine> logger)
    {
        _deliveryRepository = deliveryRepository;
        _circuitBreaker = circuitBreaker;
        _options = options.Value.Retry;
        _logger = logger;
    }

    public async Task<ProviderSendResult> ExecuteWithRetryAsync(
        Func<Task<ProviderSendResult>> sendAction,
        Guid deliveryId,
        CancellationToken cancellationToken)
    {
        var attempt = 0;

        while (attempt <= _options.MaxRetries)
        {
            attempt++;

            var state = _circuitBreaker.GetState();
            if (state == CircuitState.Open)
            {
                _logger.LogWarning("Circuit breaker is OPEN. Skipping delivery {DeliveryId}", deliveryId);
                return new ProviderSendResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Circuit breaker is open",
                    ErrorCode = "CIRCUIT_OPEN"
                };
            }

            var delayMs = CalculateBackoff(attempt);

            if (attempt > 1)
            {
                _logger.LogInformation("Retry attempt {Attempt}/{MaxRetries} for delivery {DeliveryId} (delay: {Delay}ms)",
                    attempt, _options.MaxRetries + 1, deliveryId, delayMs);

                await Task.Delay(delayMs, cancellationToken);
            }

            try
            {
                var result = await sendAction();

                if (result.IsSuccess)
                {
                    _circuitBreaker.RecordSuccess();
                    await RecordRetry(deliveryId, attempt, result, cancellationToken);
                    return result;
                }

                _circuitBreaker.RecordFailure();
                await RecordRetry(deliveryId, attempt, result, cancellationToken);

                _logger.LogWarning("Attempt {Attempt} failed for delivery {DeliveryId}: {Error}",
                    attempt, deliveryId, result.ErrorMessage);
            }
            catch (Exception ex)
            {
                _circuitBreaker.RecordFailure();
                await RecordRetry(deliveryId, attempt, new ProviderSendResult
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message,
                    ErrorCode = "RETRY_EXCEPTION"
                }, cancellationToken);

                _logger.LogError(ex, "Attempt {Attempt} threw exception for delivery {DeliveryId}", attempt, deliveryId);
            }
        }

        return new ProviderSendResult
        {
            IsSuccess = false,
            ErrorMessage = $"Max retries ({_options.MaxRetries}) exceeded",
            ErrorCode = "MAX_RETRIES_EXCEEDED"
        };
    }

    public async Task RetryFailedDeliveriesAsync(CancellationToken cancellationToken)
    {
        var failedDeliveries = await _deliveryRepository.GetFailedDeliveriesAsync(_options.MaxRetries, cancellationToken);

        foreach (var delivery in failedDeliveries)
        {
            _logger.LogInformation("Re-queuing failed delivery {DeliveryId} for retry", delivery.Id);
            delivery.Status = NotificationStatus.Queued;
            _deliveryRepository.Update(delivery);
        }
    }

    private async Task RecordRetry(Guid deliveryId, int attemptNumber, ProviderSendResult result,
        CancellationToken cancellationToken)
    {
        try
        {
            var retry = new NotificationRetry
            {
                Id = Guid.NewGuid(),
                DeliveryId = deliveryId,
                AttemptNumber = attemptNumber,
                AttemptedAt = DateTime.UtcNow,
                Status = result.IsSuccess ? NotificationStatus.Sent : NotificationStatus.Failed,
                FailureReason = result.ErrorMessage,
                IsFinal = !result.IsSuccess && attemptNumber >= _options.MaxRetries + 1
            };

            var delivery = await _deliveryRepository.GetByIdAsync(deliveryId, cancellationToken);
            if (delivery is not null)
            {
                delivery.Retries.Add(retry);
                delivery.AttemptCount = attemptNumber;
                _deliveryRepository.Update(delivery);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to record retry for delivery {DeliveryId}", deliveryId);
        }
    }

    private int CalculateBackoff(int attempt)
    {
        if (attempt <= 1) return 0;

        var delay = (int)(_options.BaseDelayMs * Math.Pow(_options.BackoffMultiplier, attempt - 2));

        delay = Math.Min(delay, _options.MaxDelayMs);

        if (_options.JitterEnabled)
        {
            var jitter = Random.Shared.Next(0, _options.JitterMaxMs + 1);
            delay += jitter;
        }

        return delay;
    }
}
